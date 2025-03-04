# 04-Simple7Segment-mdinc
# 7-Segment-Anzeige mit Klappmechanismus in Unity

Dieses Projekt simuliert eine mechanische 7-Segment-Anzeige, bei der die Segmente bei der Anzeige von Zahlen auf eine Art und Weise rotieren, als ob sie umgeklappt werden. Die Segmente drehen sich je nach ihrer Position (waagerecht oder senkrecht) um bestimmte Achsen.  

## Funktionsweise

- **Jede Zahl (0-9)** hat eine spezifische Konfiguration von **aktiven Segmenten**. Diese Konfiguration wird in einem Dictionary gespeichert.
- **Wenn eine Zahl gedrückt wird**, werden alle Segmente zuerst zurückgesetzt (ursprüngliche rotation), um ihre Ausgangsposition zu erreichen.
- **Dann werden die Segmente** für die neue Zahl rotiert. Senkrechte Segmente drehen sich um die Y-Achse, waagerechte um die X-Achse.

### **Wichtige Funktionen:**

1. **Reset & Setze Zahl**:
    - Bevor eine neue Zahl angezeigt wird, wird zuerst **alle Segmente zurückgesetzt**, um die Ursprungsposition zu erreichen.
    - Danach werden die **Segmente für die neue Zahl rotiert**.

2. **Rotation der Segmente**:
    - **Senkrechte Segmente**: Rotieren um die **Y-Achse** um 90 Grad.
    - **Waagerechte Segmente**: Rotieren um die **X-Achse** um 90 Grad.
    - Jede Drehung ist eine **Übergangsbewegung** (mittels `Quaternion.Slerp()`), die über eine definierte Zeit läuft.

### **Code Explanation:**

1. **Start()**: Initialisiert alle Segment-Referenzen und speichert die Ausgangsrotation jedes Segments.
2. **Update()**: Überwacht, ob eine Zahl (`0-9`) gedrückt wird. Wenn eine Zahl gedrückt wird, wird der **Reset- und Setze-Vorgang** gestartet.
3. **ResetAndSetDigit()**: Führt den **Reset der Segmente** und das Setzen der neuen Zahl aus.
4. **ResetSegments()**: Setzt alle Segmente auf ihre Ausgangsrotation zurück.
5. **SetDigit()**: Setzt die Segmente gemäß der angegebenen Zahl.
6. **RotateSegment()**: Führt die sanfte Drehung eines Segments aus.
7. **GetTargetRotation()**: Bestimmt, welche Achse (X oder Y) für die Rotation des Segments verwendet wird.

### **Steuerung:**

- **Zahlen 0 bis 9** können über die **Zifferntasten** auf der Tastatur eingegeben werden, um die Zahl auf der Anzeige darzustellen.

