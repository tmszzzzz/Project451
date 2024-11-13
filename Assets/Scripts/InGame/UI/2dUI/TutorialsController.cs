using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class TutorialsController : MonoBehaviour
{
    [SerializeField] private Book book;
    [SerializeField] private GameObject button1;
    [SerializeField] private GameObject button2;
    [SerializeField] private GameObject button3;
    [SerializeField] private GameObject button4;

    [SerializeField] private int page1;
    [SerializeField] private int page2;
    [SerializeField] private int page3;
    [SerializeField] private int page4;

    [SerializeField] private List<TimelineAsset> timelines;
    [SerializeField] private PlayableDirector tutorialsDirector;
    [SerializeField] private GameObject tutorialsPanel;
    [SerializeField] private AnimationClip tutorialsPanelAppear;
    [SerializeField] private AnimationClip tutorialsPanelDisappear;
    [SerializeField] private GameObject tutorialsImage;
    
    public bool canShowTutorial4 = false;
    
    void Update()
    {
        if (book.currentPage == page1 || book.currentPage == page1 + 1)
        {
            button1.SetActive(true);
        }
        else
        {
            button1.SetActive(false);
        }
        if (book.currentPage == page2 || book.currentPage == page2 + 1)
        {
            button2.SetActive(true);
        }
        else
        {
            button2.SetActive(false);
        }
        if (book.currentPage == page3 || book.currentPage == page3 + 1)
        {
            button3.SetActive(true);
        }
        else
        {
            button3.SetActive(false);
        }
        if (canShowTutorial4 && (book.currentPage == page4 || book.currentPage == page4 + 1))
        {
            button4.SetActive(true);
        }
        else
        {
            button4.SetActive(false);
        }

        if (tutorialsPanel.activeSelf && tutorialsDirector.time >= tutorialsDirector.duration - 1)
        {
            tutorialsDirector.time -= 1;
        }
    }

    public void DisplayTimeline(int n)
    {
        tutorialsPanel.SetActive(true);
        tutorialsDirector.playableAsset = timelines[n];
        StartCoroutine(PlayTutorials());
    }
    
    private IEnumerator PlayTutorials()
    {
        // 播放tutorialsPanelAppear动画
        tutorialsPanel.GetComponent<Animator>().Play(tutorialsPanelAppear.name);

        // 等待动画播放完成，假设动画的持续时间是tutorialsPanelAppear.clip.length
        yield return new WaitForSeconds(tutorialsPanelAppear.length);

        tutorialsImage.SetActive(true);
        // 动画播放完成后开始播放时间轴
        tutorialsDirector.Play();
    }
    
    public void Forward5Seconds()
    {
        tutorialsDirector.time += 5f;
        // 确保时间不超过时间轴的最大值
        tutorialsDirector.time = Mathf.Min((float)tutorialsDirector.time, (float)tutorialsDirector.duration-1);
        tutorialsDirector.Evaluate();  // 立即更新时间轴
    }

    public void Backward5Seconds()
    {
        tutorialsDirector.time -= 5f;
        // 确保时间不低于时间轴的最小值（即0秒）
        tutorialsDirector.time = Mathf.Max((float)tutorialsDirector.time, 0f);
        tutorialsDirector.Evaluate();  // 立即更新时间轴
    }
    
    public void ResetToStart()
    {
        tutorialsDirector.time = 0f;
        tutorialsDirector.Evaluate();  // 立即更新时间轴
    }
    
    public void Off()
    {
        tutorialsDirector.time = 0;
        tutorialsImage.SetActive(false);
        StartCoroutine(Quit());
    }

    private IEnumerator Quit()
    {
        tutorialsPanel.GetComponent<Animator>().Play(tutorialsPanelDisappear.name);
        yield return new WaitForSeconds(tutorialsPanelDisappear.length);
        tutorialsPanel.SetActive(false);
        tutorialsDirector.Stop();
    }
}
