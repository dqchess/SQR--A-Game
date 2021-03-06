﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour {
	public static TimeManager myTimer = null;

	public Image timer;		// Visual representation of the timer

	private float maxTime = 10.0f;
	private float activeTime;  // real time left on timer
	private float displayTime; // actual time displayed

	private bool started;   // has the timer started?
	private bool redBar = false;   // what is the color of the bar
	private bool isBlinking = false;  // Is the timer blinking?
	//private Color myColor;

	private Color myCol;
	// Use this for initialization
	void Awake () {
		if (myTimer == null)
			myTimer = this;
		else if (myTimer != this)
			Destroy(gameObject);
		started = false;
		activeTime = 0;
		displayTime = activeTime;
		myCol = ColorPicker.COL_DICT[PlayerPrefs.GetString("Theme", "WINTER")][2];
		timer.color = myCol;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateDisplay();
		SetTimer();

		// if the player is not active don't do anything
		if (!GameManager.instance.playerTurn) {
			started = false;
			return;
		}

		if (!started) {
			started = true;
			timer.color = Color.white;
			activeTime = maxTime;
		}

		activeTime -= Time.deltaTime;
		
		if (!isBlinking && activeTime < maxTime * 0.3f) {
			StartCoroutine(Blink());
		}
		// Debug.Log(""+ activeTime);
		// if time runs out, call game over
		if (activeTime <= 0) {
			GameManager.instance.GameOver();
		}
	}

	// allow displayTime to catch up to activeTime
	void UpdateDisplay() {
		if (displayTime < activeTime) {
			displayTime += Time.deltaTime * 3;
			if (displayTime > activeTime) 
				displayTime = activeTime;
		}
		else if (displayTime > activeTime) {
			displayTime -= Time.deltaTime * 3;
			if (displayTime < activeTime) 
				displayTime = activeTime;
		} 
		
	}

	// change the image to match timer
	void SetTimer() {
		timer.fillAmount = displayTime/maxTime;
	}

	// add time to timer
	public void AddTime() {
		activeTime += maxTime * 0.1f;
		if (activeTime >= maxTime) 
			activeTime = maxTime;
	}

	// reduce time available
	public void MinusTime() {
		//iTween.ShakePosition(gameObject, iTween.Hash("x", .1, "time", 0.8));
		StartCoroutine(Wrong());
		activeTime -= maxTime * 0.1f;
		if (activeTime <= 0)
			activeTime = 0.0f;
	}


	// reset the timer to max time
	public IEnumerator ResetTime() {
		timer.color = myCol;
		// float deltaTime = maxTime - activeTime;
		activeTime = maxTime;
		while (displayTime < activeTime) {
			displayTime += maxTime/45.0f;
			SetTimer();
			//Debug.Log(displayTime);
			yield return new WaitForSeconds(0.0167f);
		}
	}

    // Sets the max time to 10 seconds
    public void ResetMax() {
        maxTime = 10.0f;
    }

    // Subtracts 0.25 seconds from max time, minimum is 5 seconds
    public void MinusMax()
    {
        float temp = maxTime - 0.25f;
        maxTime = Mathf.Clamp(temp, 5.0f, 10.0f);
    }

	// let the timer switch between red and white
	IEnumerator Blink() {
		// Debug.Log("blinking");
		isBlinking = true;
		if (redBar) {
			timer.color = Color.white;
			redBar = false;
		}
		else {
			timer.color = ColorPicker.RED;
			redBar = true;
		}
		yield return new WaitForSeconds(0.2f);
		isBlinking = false;
	}

    // Sets the color of the timer to red for a little bit
	IEnumerator Wrong() {
		timer.color = ColorPicker.RED;
		yield return new WaitForSeconds(0.5f);
		timer.color = Color.white;
	}
}
