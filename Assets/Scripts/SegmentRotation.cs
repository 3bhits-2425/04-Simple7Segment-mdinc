using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//bibliothek für Buttons
using UnityEngine.UI;

//Neuigkeiten im Code:
// yield return null;  // Wartet auf den nächsten Frame ab
//QUartanion Slerp -> berechnet eine sanfte Übergangsrotation) zwischen der aktuellen Rotation und der Zielrotation
//Quaternion.Euler ->die eine Rotation im 3D-Raum darstellt basierend auf den angegebenen Eulerwinkeln (Winkel um die X-, Y- und Z-Achsen).
//Dictionary -> Wörterbuch (ähnlich wie eine Liste, aber mit Schlüssel-Wert-Paaren)
//Coroutine -> eine Methode die ihre aufgabe stoppen unddann fortführen kann (multitasking)

public class SegmentRotation : MonoBehaviour
{
    // Segmente in waagerecht und senkrecht (für die Zahlen 0 bis 9)
    public Transform d, g, a;
    public Transform b, c, f, e;

    public GameObject cube;
    // public GameObject startButton = GameObject.FindWithTag("StartButton");
  
    
    public void spinCube()
    {   
        cube.transform.Rotate(0, 180, 0);
        // startButton.interactable = false;
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

    // Liste der Segmente (wird später benutzt, um sie alle gleichzeitig je nach zahl zu rotieren)
    private Transform[] segments;

    // Speichert die Ausgangsrotation der Segmente (um sie später zurückzusetzen damit alle Rotationen klappen)
    private Quaternion[] defaultRotations;

    // variable um zu verhindern, dass während der Rücksetzung eine neue Zahl gesetzt wird
    private bool isResetting = false;

    private void Start()
    {
        cube.transform.Rotate(0, 180, 0);
        // Initialisieren der Segment-Referenzen
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
        // Überprüfen, ob eine Zahl von 0 bis 9 gedrückt wurde
        for (int i = 0; i <= 9; i++)
        {
            // Wenn eine Zahl gedrückt wird und nicht gerade zurückgesetzt wird
            if (Input.GetKeyDown(i.ToString()) && !isResetting)
            {
                StopAllCoroutines();  // Stoppe alle laufenden rotierungen
                StartCoroutine(ResetAndSetDigit(i));  // Starte den Reset-Prozess und setze dann die Zahl
            }
        }
    }

    // Coroutine für das Zurücksetzen der Segmente und das Setzen der neuen Zahl
    IEnumerator ResetAndSetDigit(int number)
    {
        isResetting = true;

        // Schritt 1: Setze alle Segmente zurück in ihre Ausgangsposition
        yield return StartCoroutine(ResetSegments());

        // Schritt 2: Setze die neue Zahl
        SetDigit(number);

        isResetting = false;  // Rücksetzung abgeschlossen
    }

    // Setzt alle Segmente auf ihre Ausgangsposition zurück
    IEnumerator ResetSegments()
    {
        List<Coroutine> runningCoroutines = new List<Coroutine>();

        // Durchläuft alle Segmente und setzt sie zurück
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

    // Setzt die Segmente für die angegebene Zahl (z.B. 0, 1, 2, etc.)
    void SetDigit(int number)
    {
        if (digitMap.ContainsKey(number))
        {
            bool[] activeSegments = digitMap[number];  // Hole die Segmentaktivierungen für diese Zahl

            // Gehe durch alle Segmente und rotiere sie, je nach dem, ob sie aktiv sind oder nicht
            for (int i = 0; i < segments.Length; i++)
            {
                // Bestimme die Zielrotation für jedes Segment
                Quaternion targetRotation = activeSegments[i] ? GetTargetRotation(segments[i]) : defaultRotations[i];
                // Starte eine Coroutine für die Drehung
                StartCoroutine(RotateSegment(segments[i], targetRotation, 0.5f));
            }
        }
    }

    // Bestimmt die Zielrotation für jedes Segment (je nachdem, ob es senkrecht oder waagerecht ist)
    Quaternion GetTargetRotation(Transform segment)
    {
        // Senkrechte Segmente (wie f, b) werden um die Y-Achse gedreht
        if (segment == f || segment == e || segment == b || segment == c)
            return Quaternion.Euler(segment.eulerAngles.x, segment.eulerAngles.y + 90, segment.eulerAngles.z); // Y-Achse

        // Waagerechte Segmente (wie a, g, d) werden um die X-Achse gedreht
        if (segment == a || segment == g || segment == d)
            return Quaternion.Euler(segment.eulerAngles.x + 90, segment.eulerAngles.y, segment.eulerAngles.z); // X-Achse

        return segment.rotation;  // Sollte kein Segment erkannt werden, bleibt die Rotation unverändert
    }

    // Führt die Rotation für ein Segment durch (mit einer sanften Übergangsbewegung)
    IEnumerator RotateSegment(Transform segment, Quaternion targetRotation, float duration)
    {
        Quaternion startRotation = segment.rotation;  // Ausgangsrotation des Segments
        float time = 0;

        // Animation läuft über die angegebene Dauer
        while (time < duration)
        {
            // Interpoliert die Rotation zwischen der aktuellen und der Zielrotation
            segment.rotation = Quaternion.Slerp(startRotation, targetRotation, time / duration);
            time += Time.deltaTime;  // Zeitfortschritt
            yield return null;  // Wartet den nächsten Frame ab
        }

        // Stellt sicher, dass das Segment die genaue Zielrotation erreicht
        segment.rotation = targetRotation;
    }
}
