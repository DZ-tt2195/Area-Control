using UnityEngine;
using MyBox;
using System.Collections;
using System.Linq;
using System;
using Photon.Pun;
using System.Collections.Generic;

public class Card : PhotonCompatible
{

#region Setup

    public CardLayout layout { get; private set; }
    bool flipping;
    public bool vertical {get; private set;}
    public CardType thisCard { get; private set; }
    public ButtonSelect selectMe { get; private set; }
    public CardData dataFile {get; private set;}

    protected override void Awake()
    {
        base.Awake();
        this.bottomType = this.GetType();

        selectMe = GetComponent<ButtonSelect>();
        layout = GetComponent<CardLayout>();
    }
    public void AssignCard(CardData dataFile, float startingAlpha, bool vertical, Vector3 scale)
    {
        this.name = dataFile.cardName;
        this.vertical = vertical;
        Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        this.transform.localScale = Vector3.Lerp(scale, canvas.transform.localScale, 0.5f);

        this.dataFile = dataFile;
        thisCard = (CardType)Activator.CreateInstance(Type.GetType(dataFile.cardName), dataFile);
        this.layout.FillInCards(dataFile, startingAlpha, vertical);
    }

    #endregion

#region Animations

    public void MoveCardRPC(Vector3 newPos, float waitTime, Vector3 newScale)
    {
        StartCoroutine(MoveCard(newPos, waitTime, newScale));
    }

    IEnumerator MoveCard(Vector3 newPos, float waitTime, Vector3 newScale)
    {
        float elapsedTime = 0;
        Vector2 originalPos = this.transform.localPosition;
        Vector2 originalScale = this.transform.localScale;

        while (elapsedTime < waitTime)
        {
            this.transform.localPosition = Vector3.Lerp(originalPos, newPos, elapsedTime / waitTime);
            this.transform.localScale = Vector3.Lerp(originalScale, newScale, elapsedTime / waitTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        this.transform.localPosition = newPos;
    }

    public void FlipCardRPC(float newAlpha, float totalTime)
    {
        if (!flipping && this.layout.GetAlpha() != newAlpha)
            StartCoroutine(FlipCard(newAlpha, totalTime));
    }

    IEnumerator FlipCard(float newAlpha, float totalTime)
    {
        flipping = true;
        transform.localEulerAngles = new Vector3(0, 0, 0);
        float elapsedTime = 0f;

        Vector3 originalRot = this.transform.localEulerAngles;
        Vector3 newRot = new(0, 90, 0);

        while (elapsedTime < totalTime)
        {
            this.transform.localEulerAngles = Vector3.Lerp(originalRot, newRot, elapsedTime / totalTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        this.layout.FillInCards(thisCard.dataFile, newAlpha, vertical);
        elapsedTime = 0f;

        while (elapsedTime < totalTime)
        {
            this.transform.localEulerAngles = Vector3.Lerp(newRot, originalRot, elapsedTime / totalTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        this.transform.localEulerAngles = originalRot;
        flipping = false;
    }

    #endregion

}