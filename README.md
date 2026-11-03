# Endless Pixelgrid · Schoolmakerdays

Unity 6000.0.57f1 · WebGL · 1920 × 1080 quer. Mitmachstation mit Makey Makey,
einer Live-Eingabezeile und einem Raster aus 10 × 7 UI Images.

## Bedienung

EARTH halten und Kontakte berühren. Blau bedeutet Kontakt, Weiß bedeutet kein Kontakt.
Keine Toggle-Funktion, keine Glättung: Jeder Frame zeigt die vom Makey Makey als
Tastatur gemeldeten Signale. Eine elektrische Rohmessung findet nicht statt.

| Position von links | Bit | Taste |
| --- | --- | --- |
| 1 | Q6 (höchstwertig) | W |
| 2 | Q5 | A |
| 3 | Q4 | Pfeil hoch |
| 4 | Q3 | Pfeil links |
| 5 | Q2 | Pfeil runter |
| 6 | Q1 | Pfeil rechts |
| 7 | Q0 (niederwertig) | S |

- **D:** einmal pro Drücken eine Momentaufnahme übernehmen. Neue Zeile unten,
  bestehende Zeilen nach oben, älteste Zeile verwerfen. Auch leere Zeilen sind erlaubt.
- **G:** Raster leeren. Gehaltene Kontakte bleiben in der Live-Zeile sichtbar.
- D und G können auch mit den Schaltflächen bedient werden.
- Alle sieben Kontakte werden vor D gemeinsam ausgewertet. G hat bei gleichzeitigem
  D/G Vorrang. Fenster-/Fokusverlust leert die Live-Anzeige.
- Browser: „Los geht’s“ anklicken. Nach einem Fensterwechsel wieder ins Spiel klicken.
- Der Browser kann Vollbild über die Schaltfläche oben rechts aktivieren.

## Unity und Build

**Windows-Schnellstart:** `Start-Pixelgrid.cmd` doppelklicken. Der Starter öffnet
den vorhandenen lokalen Build im Standardbrowser und startet dafür einen lokalen
HTTP-Server. Python 3 muss installiert sein. Wiederholtes Starten nutzt denselben
Server. Der Starter lädt nichts von GitHub herunter; er zeigt den aktuellen Build
im Ordner `docs/`. Nach Änderungen am Unity-Code zuerst neu exportieren.

1. Dieses Repository mit Unity **6000.0.57f1** öffnen, WebGL Build Support installieren.
2. Szene `Assets/EndlessPixelGrid/EndlessPixelGrid.unity` öffnen.
3. Play drücken. Szene, Logos und UI Images sind im Editor bearbeitbar.
4. Menü **Schoolmakerdays → Build WebGL for GitHub Pages** erzeugt `docs/`.

Für einen reproduzierbaren Batch-Build:

```powershell
& 'C:/Program Files/Unity/Hub/Editor/6000.0.57f1/Editor/Unity.exe' `
  -batchmode -nographics -quit -projectPath $PWD `
  -buildTarget WebGL -executeMethod WorkshopBuild.BuildWebGL -logFile build.log
```

Das Menü **Verify FIFO and live input model** prüft Bitreihenfolge, unabhängige
Momentaufnahmen, FIFO-Überlauf und Reset mit weiterhin aktivem Kontakt.
Diese Prüfungen laufen auch vor jedem Build.

## GitHub Pages

Zielrepository: https://github.com/EventPrjs/PixelCat

`docs/` enthält den fertigen WebGL-Build. Nach Commit und Push im Repository unter
**Settings → Pages → Deploy from a branch** den gewünschten Branch und **/docs** wählen.
Die erwartete Adresse ist https://eventprjs.github.io/PixelCat/ (erst nach Aktivierung).
Komprimierung ist deaktiviert, damit keine besonderen HTTP-Header erforderlich sind.
Zum lokalen Testen HTTP verwenden, nicht die HTML-Datei direkt öffnen:

```powershell
python -m http.server 8080 --directory docs
```

Dann http://localhost:8080 aufrufen. Zum Workshop vorher mit dem tatsächlichen
Makey Makey alle sieben Kontakte, D, G und Fensterwechsel testen.

## Aufbau und Quellen

- `PixelGridModel.cs`: `bool[7]` und `bool[10,7]`, reine Datenlogik.
- `EndlessPixelGrid.cs`: Eingaben, getrennte Render-Methoden und UI-Aufbau.
- `WorkshopBuild.cs`: Szenenerstellung, Modellprüfungen, WebGL-Export.
- `Assets/WebGLTemplates/Schoolmakerdays`: Browserrahmen, Ladeanzeige, Fokus, Vollbild.
- `Assets/Resources/Branding`: vom Nutzer bereitgestellte HTL-, Maker-Faire-,
  Maker-Roboter- und smeerws-Grafiken. Markenrechte verbleiben bei ihren Inhabern.
  Das Veranstaltungslogo trägt unverändert den Aufdruck „7. November 2026“;
  dies ist keine gesonderte Datumsangabe für den Schoolmakerdays-Workshop.

Die Tastenbelegung und gehaltene Eingabe wurden aus der vorhandenen `RowPixel`-Klasse
des Projekts SCC25-V1-4Tasks übernommen. Dieses Quellprojekt bleibt unverändert.
