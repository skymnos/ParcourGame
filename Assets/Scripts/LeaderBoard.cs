using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{
    public GameObject leaderboardUI;
    public TextMeshProUGUI scoreText;
    public TMP_InputField nameInputField;


    [SerializeField] private string leaderboardID;
    [SerializeField] private string leaderboardKey;
    string memberID;

    public TextMeshProUGUI[] ranksText;
    public TextMeshProUGUI[] namesText;
    public TextMeshProUGUI[] scoresText;

    void Start()
    {
        leaderboardUI.SetActive(false);

        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (!response.success)
            {
                Debug.Log("error starting LootLocker session");

                return;
            }

            Debug.Log("successfully started LootLocker session");
        });
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SubmitScore(score);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RetrieveScores();
        }*/
    }

    public void SubmitScore()
    {
        memberID = nameInputField.text;
        nameInputField.interactable = false;
        float floatScore = float.Parse(scoreText.text);
        floatScore = Mathf.Ceil(floatScore * 1000);
        int score = (int)(floatScore); // 1000 because the score will have 3 decimals HAVE TO BE CHANGED IF NB OF DECIMAL CHANGES
        LootLockerSDKManager.SubmitScore(memberID, score, leaderboardID, (response) =>
        {
            if (response.statusCode == 200)
            {
                Debug.Log("Successful");
                RetrieveScores();
            }
            else
            {
                Debug.Log("failed: " + response.errorData);
            }
        });
    }

    private void RetrieveScores()
    {
        int count = 3;
        int playerRank = 0;


        if (memberID != null)
        {

            LootLockerSDKManager.GetMemberRank(leaderboardID, memberID, (response) =>
            {
                if (response.statusCode == 200)
                {
                    playerRank = response.rank;
                    /*ranksText[5].text = response.rank.ToString();
                    namesText[5].text = response.member_id.ToString();
                    scoresText[5].text = (response.score / 1000.0).ToString();*/

                    if (playerRank <= ranksText.Length)
                    {
                        count = ranksText.Length;

                        LootLockerSDKManager.GetScoreList(leaderboardKey, count, 0, (response) =>
                        {
                            if (response.statusCode == 200)
                            {
                                LootLockerLeaderboardMember[] scores = response.items;

                                for (int i = 0; i < scores.Length; i++)
                                {
                                    ranksText[i].text = scores[i].rank.ToString();
                                    namesText[i].text = scores[i].member_id.ToString();
                                    scoresText[i].text = (scores[i].score / 1000.0).ToString();
                                }

                                if (scores.Length < count)
                                {
                                    for (int i = scores.Length; i < count; i++)
                                    {
                                        ranksText[i].text = (i + 1).ToString();
                                        namesText[i].text = "none";
                                        scoresText[i].text = "none";
                                    }
                                }
                            }
                            else
                            {
                                Debug.Log("failed: " + response.errorData);
                            }
                        });

                        return;

                    }
                    else
                    {
                        LootLockerSDKManager.GetScoreList(leaderboardKey, count, 0, (response) =>
                        {
                            if (response.statusCode == 200)
                            {
                                LootLockerLeaderboardMember[] scores = response.items;

                                for (int i = 0; i < scores.Length; i++)
                                {
                                    ranksText[i].text = scores[i].rank.ToString();
                                    namesText[i].text = scores[i].member_id.ToString();
                                    scoresText[i].text = (scores[i].score / 1000.0).ToString();
                                }

                                if (scores.Length < count)
                                {
                                    for (int i = scores.Length; i < count; i++)
                                    {
                                        ranksText[i].text = (i + 1).ToString();
                                        namesText[i].text = "none";
                                        scoresText[i].text = "none";
                                    }
                                }
                            }
                            else
                            {
                                Debug.Log("failed: " + response.errorData);
                            }
                        });
                    }

                    int rank = response.rank;
                    int count2 = 5;
                    //int after = rank < 6 ? 0 : rank - 5;
                    int after = rank -3;
                    LootLockerSDKManager.GetScoreList(leaderboardID, count2, after, (response) =>
                    {
                        if (response.statusCode == 200)
                        {
                            LootLockerLeaderboardMember[] scores = response.items;

                            for (int i = 0; i < scores.Length; i++)
                            {
                                /*if (i == count)
                                {
                                    continue;
                                }*/

                                ranksText[i + count].text = scores[i].rank.ToString();
                                namesText[i + count].text = scores[i].member_id.ToString();
                                scoresText[i + count].text = (scores[i].score / 1000.0).ToString();
                            }
                        }
                        else
                        {
                            Debug.Log("failed: " + response.errorData);
                        }
                    });
                }
                else
                {
                    Debug.Log("failed: " + response.errorData);
                }
            });
        }
        else
        {
            count = ranksText.Length;

            LootLockerSDKManager.GetScoreList(leaderboardKey, count, 0, (response) =>
            {
                if (response.statusCode == 200)
                {
                    LootLockerLeaderboardMember[] scores = response.items;

                    for (int i = 0; i < scores.Length; i++)
                    {
                        ranksText[i].text = scores[i].rank.ToString();
                        namesText[i].text = scores[i].member_id.ToString();
                        scoresText[i].text = (scores[i].score / 1000.0).ToString();
                    }

                    if (scores.Length < count)
                    {
                        for (int i = scores.Length; i < count; i++)
                        {
                            ranksText[i].text = (i + 1).ToString();
                            namesText[i].text = "none";
                            scoresText[i].text = "none";
                        }
                    }
                }
                else
                {
                    Debug.Log("failed: " + response.errorData);
                }
            });
        }
    }

    public void LevelFinished()
    {
        leaderboardUI.SetActive(true);
        RetrieveScores();
    }

    public void ResetLevel()
    {
        leaderboardUI.SetActive(false);
    }
}
