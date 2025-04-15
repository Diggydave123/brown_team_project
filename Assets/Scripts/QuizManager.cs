using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Question model - supports both single and multiple correct answers
[System.Serializable]
public class Question
{
    public string question;
    public string[] options;
    public string correct_answer;      // used for single-answer questions
    public string[] correct_answers;   // used for multi-answer questions

    public bool IsMultiAnswer => correct_answers != null && correct_answers.Length > 0;
}

// Wrapper class for JSON deserialization
[System.Serializable]
public class QuestionList
{
    public List<Question> questions;
}

public class QuizManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject topicSelectionPanel; // initial topic selection screen
    public Toggle aiToggle, dataToggle, cyberToggle;
    public Button startQuizButton;

    public GameObject quizPanel; // main quiz display (containing both question UIs)
    public TMP_Text singleQuestionText; // text for single-answer questions
    public TMP_Text multiQuestionText;  // text for multi-answer questions

    public GameObject singleQuizPanel, multiQuizPanel; // containers for each quiz mode

    public Transform singleOptionContainer, multiOptionContainer; // where options will be spawned

    public GameObject answerButtonPrefab, answerToggleBoxPrefab; // prefabs for answers

    public Button singleContinueButton, multiContinueButton, multiSubmitButton;

    private List<GameObject> spawnedOptions = new List<GameObject>(); // holds spawned UI elements
    private QuestionList questionList;
    private int currentQuestionIndex = 0;

    public bool wasAnswerCorrect = false; // used to track if the user got it right

    void Start()
    {
        topicSelectionPanel.SetActive(true); // show topic UI on start
        quizPanel.SetActive(false);          // hide quiz initially
        startQuizButton.onClick.AddListener(StartQuiz); // bind quiz start
    }

    void StartQuiz()
    {
        List<Question> combinedQuestions = new List<Question>();

        // Load questions based on selected toggles
        if (aiToggle.isOn)
        {
            combinedQuestions.AddRange(LoadQuestionsFromJson("AI_Questions"));
            combinedQuestions.AddRange(LoadQuestionsFromJson("AI_Multiple_Choice_Questions"));
        }
        if (dataToggle.isOn)
        {
            combinedQuestions.AddRange(LoadQuestionsFromJson("Data_Questions"));
            combinedQuestions.AddRange(LoadQuestionsFromJson("Data_Multiple_Choice_Questions"));
        }
        if (cyberToggle.isOn)
        {
            combinedQuestions.AddRange(LoadQuestionsFromJson("Cybersecurity_Questions"));
            combinedQuestions.AddRange(LoadQuestionsFromJson("Cybersecurity_Multiple_Choice_Questions"));
        }

        // Safety check if nothing was selected
        if (combinedQuestions.Count == 0)
        {
            Debug.LogWarning("❌ No topic selected!");
            return;
        }

        Shuffle(combinedQuestions); // randomize questions
        questionList = new QuestionList { questions = combinedQuestions };
        currentQuestionIndex = 0;

        topicSelectionPanel.SetActive(false);
        quizPanel.SetActive(true);
        DisplayQuestion();
    }

    // Loads a JSON file from Resources
    List<Question> LoadQuestionsFromJson(string fileName)
    {
        TextAsset json = Resources.Load<TextAsset>(fileName);
        if (json == null)
        {
            Debug.LogWarning($"Missing JSON: {fileName}");
            return new List<Question>();
        }

        return JsonUtility.FromJson<QuestionList>(json.text)?.questions ?? new List<Question>();
    }

    // Fisher-Yates shuffle
    void Shuffle(List<Question> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    // Core logic for displaying each question
    void DisplayQuestion()
    {
        ClearOptions(); // remove old answers

        if (currentQuestionIndex >= questionList.questions.Count)
        {
            Debug.Log("✅ Quiz Complete!");
            return;
        }

        Question q = questionList.questions[currentQuestionIndex];

        // Set question text depending on type
        if (q.IsMultiAnswer)
            multiQuestionText.text = q.question;
        else
            singleQuestionText.text = q.question;

        // Toggle UI mode
        bool isMulti = q.IsMultiAnswer;
        singleQuizPanel.SetActive(!isMulti);
        multiQuizPanel.SetActive(isMulti);

        // Select which container/prefab to use
        Transform container = isMulti ? multiOptionContainer : singleOptionContainer;
        GameObject prefab = isMulti ? answerToggleBoxPrefab : answerButtonPrefab;

        // Spawn all answer options
        foreach (string option in q.options)
        {
            GameObject optionObj = Instantiate(prefab, container);
            TMP_Text label = optionObj.GetComponentInChildren<TMP_Text>();
            label.text = option;

            if (isMulti)
            {
                Toggle toggle = optionObj.GetComponentInChildren<Toggle>();
                toggle.isOn = false;
                toggle.interactable = true;
            }
            else
            {
                Button btn = optionObj.GetComponentInChildren<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => HandleSingleAnswer(btn, label.text, q));
            }

            spawnedOptions.Add(optionObj);
        }

        // Reset and configure button interactions
        multiContinueButton.onClick.RemoveAllListeners();
        multiSubmitButton.onClick.RemoveAllListeners();
        singleContinueButton.onClick.RemoveAllListeners();

        if (isMulti)
        {
            // Setup for multi-answer
            multiContinueButton.interactable = false;
            multiSubmitButton.interactable = true;

            multiContinueButton.gameObject.SetActive(true);
            multiSubmitButton.gameObject.SetActive(true);
            singleContinueButton.gameObject.SetActive(false);

            multiSubmitButton.onClick.AddListener(() =>
            {
                HandleMultiAnswer(q);
                multiSubmitButton.interactable = false;
                multiContinueButton.interactable = true;
            });

            multiContinueButton.onClick.AddListener(() => NextQuestion());
        }
        else
        {
            // Setup for single-answer
            singleContinueButton.interactable = false;
            multiContinueButton.gameObject.SetActive(false);
            multiSubmitButton.gameObject.SetActive(false);
            singleContinueButton.gameObject.SetActive(true);
        }
    }

    // When a single-answer question is answered
    void HandleSingleAnswer(Button selectedBtn, string selectedText, Question q)
    {
        wasAnswerCorrect = selectedText == q.correct_answer;
        Debug.Log($"wasAnswerCorrect = {wasAnswerCorrect}");

        foreach (GameObject obj in spawnedOptions)
        {
            Button btn = obj.GetComponentInChildren<Button>();
            TMP_Text label = btn.GetComponentInChildren<TMP_Text>();
            Image bg = obj.GetComponent<Image>();

            bool isCorrect = label.text == q.correct_answer;
            if (isCorrect)
                bg.color = Color.green;
            else if (label.text == selectedText)
                bg.color = Color.red;

            btn.interactable = false;
        }

        singleContinueButton.onClick.RemoveAllListeners();
        singleContinueButton.onClick.AddListener(() => NextQuestion());
        singleContinueButton.interactable = true;
    }

    // When a multi-answer question is submitted
    void HandleMultiAnswer(Question q)
    {
        List<string> selected = new List<string>();
        foreach (var obj in spawnedOptions)
        {
            Toggle toggle = obj.GetComponentInChildren<Toggle>();
            if (toggle != null && toggle.isOn)
            {
                selected.Add(toggle.GetComponentInChildren<TMP_Text>().text);
            }
        }

        HashSet<string> correctSet = new HashSet<string>(q.correct_answers);
        wasAnswerCorrect = correctSet.SetEquals(selected);
        Debug.Log($"wasAnswerCorrect = {wasAnswerCorrect}");

        foreach (var obj in spawnedOptions)
        {
            Toggle toggle = obj.GetComponentInChildren<Toggle>();
            TMP_Text label = toggle.GetComponentInChildren<TMP_Text>();
            Image bg = obj.GetComponent<Image>();

            bool isCorrect = correctSet.Contains(label.text);
            if (isCorrect)
                bg.color = Color.green;
            else if (toggle.isOn)
                bg.color = Color.red;

            toggle.interactable = false;
        }

        // Waits for user to hit Continue now
    }

    // Optional coroutine if you want auto-advance (not currently used)
    IEnumerator AutoNextQuestion()
    {
        yield return new WaitForSeconds(5f);
        NextQuestion();
    }

    // Move to next question
    void NextQuestion()
    {
        currentQuestionIndex++;
        DisplayQuestion();
    }

    // Destroy all dynamically spawned buttons/toggles
    void ClearOptions()
    {
        foreach (var obj in spawnedOptions)
        {
            Destroy(obj);
        }
        spawnedOptions.Clear();
    }
}














