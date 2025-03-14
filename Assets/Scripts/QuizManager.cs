using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Import TextMeshPro
using UnityEngine.UI; // Keep UI for Buttons
using System.IO; // Required for JSON handling

// Data structure for a single question
[System.Serializable]
public class Question
{
    public string question;    // The question text
    public string[] options;   // Array of possible answers
    public string correct_answer; // The correct answer
}

// Data structure to store a list of questions
[System.Serializable]
public class QuestionList
{
    public List<Question> questions;
}

public class QuizManager : MonoBehaviour
{
    public TMP_Text QuestionText; // Use TMP_Text instead of Text
    public Button[] optionButtons; // Array to hold answer buttons
    public Text feedbackText; // UI Text to show correct/wrong feedback (optional)
    public Text scoreText; // UI Text to display score
    public GameObject quizPanel; // Panel containing quiz UI
    public GameObject resultPanel; // Panel to show final result

    private QuestionList questionList; // Stores all quiz questions
    private int currentQuestionIndex = 0; // Track current question
    private int score = 0; // Keep track of the score

    void Start()
    {
        LoadQuestions(); // Load JSON questions
        DisplayQuestion(); // Show first question
    }

    // Load questions from JSON file
    void LoadQuestions()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("questions"); // Load JSON from Resources folder

        if (jsonFile == null)
        {
            Debug.LogError("🚨 ERROR: JSON file not found! Make sure 'questions.json' is inside 'Assets/Resources'.");
            return;
        }

        questionList = JsonUtility.FromJson<QuestionList>(jsonFile.text); // Convert JSON into QuestionList

        if (questionList == null || questionList.questions.Count == 0)
        {
            Debug.LogError("🚨 ERROR: JSON file loaded but contains no questions!");
        }
        else
        {
            Debug.Log("✅ JSON Loaded Successfully! Questions found: " + questionList.questions.Count);
        }
    }


    // Display the current question on the UI
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
            // Reset button color to default
            ColorBlock cb = optionButtons[i].colors;
            cb.normalColor = Color.white; // Default color (adjust if needed)
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
        CheckAnswer(optionButtons[buttonIndex].GetComponentInChildren<TMP_Text>().text,
                    questionList.questions[currentQuestionIndex].correct_answer,
                    clickedButton);
    }



    // Check if selected answer is correct
   void CheckAnswer(string selected, string correct, Button clickedButton)
    {
        ColorBlock cb = clickedButton.colors; // Get button color settings

        if (selected == correct)
        {
            Debug.Log("✅ Correct Answer!");
            cb.normalColor = Color.green; // Change button to green
            cb.selectedColor = Color.green;
        }
        else
        {
            Debug.Log("❌ Wrong Answer!");
            cb.normalColor = Color.red; // Change button to red
            cb.selectedColor = Color.red;
        }

        clickedButton.colors = cb; // Apply color change

        currentQuestionIndex++; // Move to next question
        Invoke("DisplayQuestion", 1.5f); // Wait 1.5 seconds before showing next question
    }



    // End the quiz and show final score
    void EndQuiz()
    {
        quizPanel.SetActive(false); // Hide quiz UI
        resultPanel.SetActive(true); // Show result UI
        resultPanel.GetComponentInChildren<Text>().text = "Final Score: " + score + " / " + questionList.questions.Count;
    }
}


