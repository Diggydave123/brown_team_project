using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem.LowLevel;

// --------------------
// Data Models
// --------------------

// Defines a Question, supporting both single and multi-answer types
[System.Serializable]
public class Question
{
    public string question;                // The text of the question
    public string[] options;               // All possible answer options
    public string correct_answer;          // Single correct answer (used for single-answer)
    public string[] correct_answers;       // Multiple correct answers (used for multi-answer)

    // Property to check if this is a multi-answer question
    public bool IsMultiAnswer => correct_answers != null && correct_answers.Length > 0;
}

// Wrapper class to hold a list of questions when deserializing JSON
[System.Serializable]
public class QuestionList
{
    public List<Question> questions;
}

// --------------------
// Main Quiz Manager Logic
// --------------------

public class QuizManager : MonoBehaviour
{
    // UI references set in the Inspector
    [Header("UI References")]
    public GameObject topicSelectionPanel;           // Panel to pick quiz topics
    public Toggle aiToggle, dataToggle, cyberToggle; // Topic toggles
    public Button startQuizButton;                  // Button to start quiz

    public GameObject quizPanel;                   // Panel containing quiz interface
    public TMP_Text singleQuestionText;           // Text for single-answer questions
    public TMP_Text multiQuestionText;           // Text for multi-answer questions

    public GameObject singleQuizPanel, multiQuizPanel;    // Panels for single and multi modes
    public Transform singleOptionContainer, multiOptionContainer; // Parents where options spawn
    public GameObject answerButtonPrefab, answerToggleBoxPrefab; // Prefabs for button/toggle

    public Button singleContinueButton, multiContinueButton, multiSubmitButton; // Action buttons

    private List<GameObject> spawnedOptions = new List<GameObject>(); // Holds spawned answer buttons/toggles
    private QuestionList questionList;               // Loaded question set
    private int currentQuestionIndex = 0;           // Index of current question

    public bool wasAnswerCorrect = false;           // Tracks correctness of last answer

    public static QuizManager singleton;
    public System.Action<bool> OnQuestionFinished; // bool = wasAnswerCorrect

    void Awake()
    {
        singleton = this;
    }

    void Start()
    {
        topicSelectionPanel.SetActive(false);         // Show topic screen on start
        // topicSelectionPanel.SetActive(true);         // Show topic screen on start
        quizPanel.SetActive(false);                 // Hide quiz interface

        startQuizButton.onClick.AddListener(StartQuiz); // Link start button to start quiz logic
    }

    void StartQuiz()
    {
        List<Question> combinedQuestions = new List<Question>();

        // Load selected topics from JSON
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

        // Show warning if no topics selected
        if (combinedQuestions.Count == 0)
        {
            Debug.LogWarning("❌ No topic selected!");
            return;
        }

        Shuffle(combinedQuestions); // Randomize question order

        questionList = new QuestionList { questions = combinedQuestions };
        currentQuestionIndex = 0;

        topicSelectionPanel.SetActive(false);
        quizPanel.SetActive(true);

        DisplayQuestion(); // Show first question
    }

    // Loads questions from a JSON file in Resources
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

    // Shuffles the question list randomly (Fisher-Yates algorithm)
    void Shuffle(List<Question> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    // Displays current question and dynamically generates UI
    void DisplayQuestion()
    {
        ClearOptions(); // Remove old answer buttons/toggles

        if (currentQuestionIndex >= questionList.questions.Count)
        {
            Debug.Log("✅ Quiz Complete!");
            return;
        }

        Question q = questionList.questions[currentQuestionIndex];

        // Show question text in correct panel
        if (q.IsMultiAnswer)
            multiQuestionText.text = q.question;
        else
            singleQuestionText.text = q.question;

        // Toggle between single or multi mode
        bool isMulti = q.IsMultiAnswer;
        singleQuizPanel.SetActive(!isMulti);
        multiQuizPanel.SetActive(isMulti);

        // Choose container and prefab type
        Transform container = isMulti ? multiOptionContainer : singleOptionContainer;
        GameObject prefab = isMulti ? answerToggleBoxPrefab : answerButtonPrefab;

        // Dynamically create answer options
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

        // Reset and wire up Continue/Submit buttons
        multiContinueButton.onClick.RemoveAllListeners();
        multiSubmitButton.onClick.RemoveAllListeners();
        singleContinueButton.onClick.RemoveAllListeners();

        if (isMulti)
        {
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
            singleContinueButton.interactable = false;
            multiContinueButton.gameObject.SetActive(false);
            multiSubmitButton.gameObject.SetActive(false);
            singleContinueButton.gameObject.SetActive(true);
        }
    }

    // Process single-answer click
    void HandleSingleAnswer(Button selectedBtn, string selectedText, Question q)
    {
        wasAnswerCorrect = selectedText == q.correct_answer;
        Debug.Log($"wasAnswerCorrect = {wasAnswerCorrect}");

        // Mark correct and incorrect buttons
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

        // Enable Continue button
        singleContinueButton.onClick.RemoveAllListeners();
        singleContinueButton.onClick.AddListener(() => NextQuestion());
        singleContinueButton.interactable = true;
    }

    // Process multi-answer submit
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

        // Mark correct and incorrect toggles
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
    }

    // Optional coroutine to auto-advance (currently unused)
    IEnumerator AutoNextQuestion()
    {
        yield return new WaitForSeconds(5f);
        NextQuestion();
    }

    // Advance to next question
    void NextQuestion()
    {
        currentQuestionIndex++;

        // Hide quiz panel after question
        quizPanel.SetActive(false);

        // Notify game logic with result
        OnQuestionFinished?.Invoke(wasAnswerCorrect);
        
        //DisplayQuestion();
    }

    // Clear all dynamically created options
    void ClearOptions()
    {
        foreach (var obj in spawnedOptions)
        {
            Destroy(obj);
        }
        spawnedOptions.Clear();
    }

    // CODE TO FUNCTION WITH GAME SPELL CARDS:
    public void TriggerNextQuestionFromGame()
    {
        if (questionList == null || questionList.questions.Count == 0)
        {
            List<Question> combinedQuestions = new List<Question>();
            combinedQuestions.AddRange(LoadQuestionsFromJson("AI_Questions"));
            combinedQuestions.AddRange(LoadQuestionsFromJson("AI_Multiple_Choice_Questions"));

            combinedQuestions.AddRange(LoadQuestionsFromJson("Data_Questions"));
            combinedQuestions.AddRange(LoadQuestionsFromJson("Data_Multiple_Choice_Questions"));

            combinedQuestions.AddRange(LoadQuestionsFromJson("Cybersecurity_Questions"));
            combinedQuestions.AddRange(LoadQuestionsFromJson("Cybersecurity_Multiple_Choice_Questions"));
            Shuffle(combinedQuestions);

            questionList = new QuestionList { questions = combinedQuestions };
            currentQuestionIndex = 0;

        }

        if (currentQuestionIndex < questionList.questions.Count)
        {
            quizPanel.SetActive(true);
            DisplayQuestion();
        }
        else
        {
            Debug.Log("✅ No more quiz questions.");
            quizPanel.SetActive(false);
        }
    }
}
