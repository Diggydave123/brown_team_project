using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class Question
{
    public string question;
    public string[] options;
    public string correct_answer;
}

[System.Serializable]
public class QuestionList
{
    public List<Question> questions;
}

public class QuizManager : MonoBehaviour
{
    public TMP_Text QuestionText;
    public Button[] optionButtons;
    public Text feedbackText;
    public Text scoreText;
    public GameObject quizPanel;

    public GameObject topicSelectionPanel;
    public Toggle aiToggle;
    public Toggle cyberToggle;
    public Toggle dataToggle;
    public Button startQuizButton;
    public bool wasAnswerCorrect = false; // resets with each question


    private QuestionList questionList;
    private int currentQuestionIndex = 0;
    private int score = 0;

    void Start()
    {
        topicSelectionPanel.SetActive(true);
        quizPanel.SetActive(false);

        startQuizButton.onClick.AddListener(OnStartQuizClicked);
    }

    void OnStartQuizClicked()
    {
        List<Question> combinedQuestions = new List<Question>();

        if (aiToggle.isOn)
            combinedQuestions.AddRange(LoadQuestionsFromJson("AI_Questions"));

        if (cyberToggle.isOn)
            combinedQuestions.AddRange(LoadQuestionsFromJson("Cybersecurity_Questions"));

        if (dataToggle.isOn)
            combinedQuestions.AddRange(LoadQuestionsFromJson("Data_Questions"));

        if (combinedQuestions.Count == 0)
        {
            Debug.LogWarning("No topics selected.");
            return;
        }

        Shuffle(combinedQuestions);

        questionList = new QuestionList { questions = combinedQuestions };
        currentQuestionIndex = 0;
        score = 0;

        topicSelectionPanel.SetActive(false);
        quizPanel.SetActive(true);
        DisplayQuestion();
    }

    List<Question> LoadQuestionsFromJson(string fileName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);
        if (jsonFile == null)
        {
            Debug.LogWarning($"Could not find {fileName}.json in Resources!");
            return new List<Question>();
        }

        QuestionList loaded = JsonUtility.FromJson<QuestionList>(jsonFile.text);
        return loaded?.questions ?? new List<Question>();
    }

    void Shuffle(List<Question> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            Question temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }

    void DisplayQuestion()
    {
        if (currentQuestionIndex >= questionList.questions.Count)
        {
            EndQuiz();
            return;
        }

        Question q = questionList.questions[currentQuestionIndex];
        QuestionText.text = q.question;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            ColorBlock cb = optionButtons[i].colors;
            cb.normalColor = Color.white;
            cb.selectedColor = Color.white;
            optionButtons[i].colors = cb;

            optionButtons[i].GetComponentInChildren<TMP_Text>().text = q.options[i];
            optionButtons[i].onClick.RemoveAllListeners();

            int buttonIndex = i;
            optionButtons[i].onClick.AddListener(() => OnButtonClick(buttonIndex, optionButtons[buttonIndex]));
        }
    }

    void OnButtonClick(int buttonIndex, Button clickedButton)
    {
        string selected = optionButtons[buttonIndex].GetComponentInChildren<TMP_Text>().text;
        string correct = questionList.questions[currentQuestionIndex].correct_answer;

        ColorBlock cb = clickedButton.colors;

        if (selected == correct)
        {
            wasAnswerCorrect = true;
            Debug.Log("✅ Correct Answer!");
            cb.normalColor = Color.green;
            cb.selectedColor = Color.green;
        }
        else
        {
            wasAnswerCorrect = false;
            Debug.Log("❌ Wrong Answer!");
            cb.normalColor = Color.red;
            cb.selectedColor = Color.red;
        }

        clickedButton.colors = cb;
        currentQuestionIndex++;
        Invoke("DisplayQuestion", 1.5f);
    }

void EndQuiz()
{
    // Just update the score text
    if (scoreText != null)
    {
        scoreText.text = $"Final Score: {score} / {questionList.questions.Count}";
    }

    Debug.Log($"✅ Quiz Complete! Score: {score} / {questionList.questions.Count}");
}

}


