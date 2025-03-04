using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SevenSegmentDisplay : MonoBehaviour
{
    public Transform wunten, wmitte, woben;
    public Transform srechts, srechtsu, slinks, slinksu;

    private Dictionary<int, bool[]> digitMap = new Dictionary<int, bool[]>()
    {
        { 0, new bool[] { true, false, true, true, true, true, true } },  // 0
        { 1, new bool[] { false, false, false, false, true, false, true } },  // 1
        { 2, new bool[] { true, true, true, false, true, true, false } },  // 2
        { 3, new bool[] { true, true, true, false, true, false, true } },  // 3
        { 4, new bool[] { false, true, false, true, true, false, true } },  // 4
        { 5, new bool[] { true, true, true, true, false, false, true } },  // 5
        { 6, new bool[] { true, true, true, true, false, true, true } },  // 6
        { 7, new bool[] { true, false, false, false, true, false, true } },  // 7
        { 8, new bool[] { true, true, true, true, true, true, true } },  // 8
        { 9, new bool[] { true, true, true, true, true, false, true } }   // 9
    };

    private Transform[] segments;
    private Quaternion[] defaultRotations;
    private bool isResetting = false;

    void Start()
    {
        segments = new Transform[] { woben, wmitte, wunten, slinks, srechts, slinksu, srechtsu };

        // Speichert die Ausgangsrotationen der Segmente
        defaultRotations = new Quaternion[segments.Length];
        for (int i = 0; i < segments.Length; i++)
        {
            defaultRotations[i] = segments[i].rotation;
        }
    }

    void Update()
    {
        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown(i.ToString()) && !isResetting)
            {
                StopAllCoroutines();  // Stoppe alte Animationen
                StartCoroutine(ResetAndSetDigit(i));
            }
        }
    }

    IEnumerator ResetAndSetDigit(int number)
    {
        isResetting = true;

        // Schritt 1: Zurücksetzen der Segmente
        yield return StartCoroutine(ResetSegments());

        // Schritt 2: Neue Zahl setzen
        SetDigit(number);

        isResetting = false;
    }

    IEnumerator ResetSegments()
    {
        List<Coroutine> runningCoroutines = new List<Coroutine>();

        for (int i = 0; i < segments.Length; i++)
        {
            runningCoroutines.Add(StartCoroutine(RotateSegment(segments[i], defaultRotations[i], 0.3f)));
        }

        foreach (Coroutine runningCoroutine in runningCoroutines)
        {
            yield return runningCoroutine;
        }
    }

    void SetDigit(int number)
    {
        if (digitMap.ContainsKey(number))
        {
            bool[] activeSegments = digitMap[number];

            for (int i = 0; i < segments.Length; i++)
            {
                Quaternion targetRotation = activeSegments[i] ? GetTargetRotation(segments[i]) : defaultRotations[i];
                StartCoroutine(RotateSegment(segments[i], targetRotation, 0.5f));
            }
        }
    }

    Quaternion GetTargetRotation(Transform segment)
    {
        if (segment == slinks || segment == slinksu || segment == srechts || segment == srechtsu)
            return Quaternion.Euler(segment.eulerAngles.x, segment.eulerAngles.y + 90, segment.eulerAngles.z); // Y-Achse für senkrechte

        if (segment == woben || segment == wmitte || segment == wunten)
            return Quaternion.Euler(segment.eulerAngles.x + 90, segment.eulerAngles.y, segment.eulerAngles.z); // X-Achse für waagerechte

        return segment.rotation;
    }

    IEnumerator RotateSegment(Transform segment, Quaternion targetRotation, float duration)
    {
        Quaternion startRotation = segment.rotation;
        float time = 0;

        while (time < duration)
        {
            segment.rotation = Quaternion.Slerp(startRotation, targetRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        segment.rotation = targetRotation;  // Stellt sicher, dass es genau passt
    }
}
