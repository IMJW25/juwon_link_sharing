using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public TalkManager talkManager;
    public GameObject talkPanel;
    public TextMeshProUGUI TalkText;
    public GameObject scanObject;
    public bool isAction; 
    public int talkIndex;
    public GameObject choicePanel;
    public GameObject passwordChangePanel; // 비밀번호 변경 UI 패널 (Canvas 안 UI)
    public DoorLockChanger doorLockChanger; // 비밀번호 변경 처리 스크립트
    public GameObject PuzzleScene;
    [HideInInspector]
    public bool isPuzzleMode = false;
    public bool isPuzzleCompleted = false;



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (isAction && Input.GetKeyDown(KeyCode.Space)) // 스페이스바 누르면 대사창 활성화
        {
            Talk(scanObject.GetComponent<ObjData>().id, scanObject.GetComponent<ObjData>().isNpc);

            if (!isAction)
            {
                isAction = false; // 대화가 종료되면 isAction을 false로 설정
            }

        }
    }

    public void Action(GameObject scanObj) // 조사액션 시 대화 실행
    {
        scanObject = scanObj;
        ObjData objData = scanObject.GetComponent<ObjData>();

        Debug.Log("대화 실행!");

        // 특정 id(100)일 경우 바로 choicePanel 활성화
        if (objData.id == 100 && choicePanel != null)
        {
            choicePanel.SetActive(true);
            isAction = true;
            return;
        }

        if (objData.id == 2000)
        {
            Debug.Log("🚪 DoorLock 씬으로 이동!");
            SceneManager.LoadScene("DoorLock");
            return;
        }

        if (objData.id == 2003 && passwordChangePanel != null)
        {
            Debug.Log("🔐 비밀번호 변경 패널 열기");
            passwordChangePanel.SetActive(true);
            doorLockChanger.ShowUIOnce(); // 비밀번호 변경 UI 열기 (한 번만)
            isAction = true;
            return;
        }

        if (objData.id == 3000)
        {
            if (PuzzleScene != null && !PuzzleScene.activeSelf)
            {
                PuzzleScene.SetActive(true);
                isPuzzleMode = true;

                if (isPuzzleCompleted)
                {
                    Debug.Log("이미 풀이 완료된 퍼즐입니다!");
                    // 안내 팝업/사운드 추가 가능
                }

                Debug.Log($"{PuzzleScene.name}이(가) 활성화됨");
            }
            isAction = false;
            return;
        }


        talkIndex = 0;
        isAction = true;
        talkPanel.SetActive(true);
    }

    void Talk(int id, bool isNpc)
    {
        string talkData = talkManager.GetTalk(id, talkIndex);
        Debug.Log($"Talk Index: {talkIndex}, Talk Data: {talkData}");

        Debug.Log($"talkPanel 상태: {talkPanel} / 활성화 여부: {talkPanel?.activeSelf}");

        if (talkData == null)
        {
            talkPanel.SetActive(false);
            Debug.Log("📢 대화 종료!");
            isAction = false;
            talkIndex = 0;
            return;
        }

        TalkText.text = talkData;
        isAction = true;
        talkIndex++;
    }

    public void CloseChoicePanel()
    {
        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }
        isAction = false;
    }
}