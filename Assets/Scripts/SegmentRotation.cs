using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SegmentRotation : MonoBehaviour
{
    // Segmente in waagerecht und senkrecht (für die Zahlen 0 bis 9)
    public Transform d, g, a;
    public Transform b, c, f, e;
    public int alleZahlen = 9;
    public GameObject cube;

    public void spinCube()
    {   
        cube.transform.Rotate(0, 180, 0);
    }

    // Speichert welche Segmente für jede Zahl 0bis 9 aktiv sind -> Segmentanordnung für jede Zahl
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

    private void Start()
    {
        cube.transform.Rotate(0, 180, 0);
        segments = new Transform[] { a, g, d, f, b, e, c };

        // Speichert die Ausgangsrotationen der Segmente
        defaultRotations = new Quaternion[segments.Length];
        for (int i = 0; i < segments.Length; i++)
        {
            defaultRotations[i] = segments[i].rotation;
        }
    }

    private void Update()
    {
        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown(i.ToString()) && !isResetting)
            {
                StopAllCoroutines(); 
                StartCoroutine(ResetAndSetDigit(i)); 
            }
        }
    }

    // Coroutine für das Zurücksetzen der Segmente und das Setzen der neuen Zahl
    IEnumerator ResetAndSetDigit(int number)
    {
        isResetting = true;
        yield return StartCoroutine(ResetSegments());
        SetDigit(number);
        isResetting = false; 
    }

    IEnumerator ResetSegments()
    {
        List<Coroutine> runningCoroutines = new List<Coroutine>();
        for (int i = 0; i < segments.Length; i++)
        {
            // Startet für jedes Segment eine Coroutine, die es zurückdreht
            runningCoroutines.Add(StartCoroutine(RotateSegment(segments[i], defaultRotations[i], 0.3f)));
        }

        // Wartet, bis alle Segmente zurückgesetzt sind
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
        if (segment == f || segment == e || segment == b || segment == c)
            return Quaternion.Euler(segment.eulerAngles.x, segment.eulerAngles.y + 90, segment.eulerAngles.z); // Y-Achse

        if (segment == a || segment == g || segment == d)
            return Quaternion.Euler(segment.eulerAngles.x + 90, segment.eulerAngles.y, segment.eulerAngles.z); // X-Achse

        return segment.rotation; 
    }

    IEnumerator RotateSegment(Transform segment, Quaternion targetRotation, float duration)
    {
        Quaternion startRotation = segment.rotation; 
        float time = 0;

        while (time < duration)
        {
            segment.rotation = Quaternion.Slerp(startRotation, targetRotation, time / duration);
            time += Time.deltaTime;  // Zeitfortschritt
            yield return null;  // Wartet den nächsten Frame ab
        }
        segment.rotation = targetRotation;
    }
} 
