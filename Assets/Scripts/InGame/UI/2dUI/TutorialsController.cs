using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

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
        if (book.currentPage == page4 || book.currentPage == page4 + 1)
        {
            button4.SetActive(true);
        }
        else
        {
            button4.SetActive(false);
        }
    }

    public void DisplayTimeline(int n)
    {
        tutorialsDirector.playableAsset = timelines[n];
    }
}
