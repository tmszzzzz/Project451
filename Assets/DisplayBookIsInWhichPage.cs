using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayBookIsInWhichPage : MonoBehaviour
{
    public Book book;

    public TextMeshProUGUI txt;
    void Update()
    {
        txt.text = $"{book.currentPage}";
    }
}
