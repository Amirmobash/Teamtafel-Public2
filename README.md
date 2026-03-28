# Teamtafel-Public

**Teamtafel** is a WPF-based workforce management dashboard for shift planning, absence tracking, task management, and report generation — designed for clarity, speed, and shared-folder workflows.

**Author:** Amir Mobasheraghdam (Amir Mobasheraghdam)

---

## 🇩🇪 Deutsch

### Überblick
**Teamtafel** ist ein WPF-basiertes Dashboard zur Personal- und Schichtverwaltung. Es unterstützt die Planung von Schichten, die Erfassung von Abwesenheiten, die Verwaltung von Aufgaben sowie die Erstellung von Berichten. Die Datenhaltung erfolgt über JSON-Dateien in einem gemeinsamen Datenordner, wodurch einfache Multi-PC-Workflows möglich sind.

### Highlights

#### ✅ Mitarbeiterverwaltung
- Mitarbeiter hinzufügen, bearbeiten und löschen  
- Rollen zuweisen und Aufsichten/Manager verwalten  
- Optional: Mitarbeiterfotos

#### ✅ Schichtplanung
- Morgen-/Abend-/Nachtschichten (je nach Implementierung/Config)
- Konfigurierbare Schichtkapazität  
- Drag & Drop-Zuweisung (wo implementiert)  
- Schichten tauschen und Slots leeren

#### ✅ Abwesenheiten
- Urlaub, Krank, Unentschuldigt  
- Tageslisten nach Kategorien gruppiert  
- Verhindert das Zuweisen abwesender Mitarbeiter zu Schichten

#### ✅ Aufgabenverwaltung
- Aufgaben erstellen, aktualisieren, löschen  
- Status: **Ausstehend / In Bearbeitung / Abgeschlossen**  
- Mitarbeiter Aufgaben zuweisen  
- Fortschritt und Notizen erfassen (wo implementiert)

#### ✅ Reporting
- Tages-/Wochen-/Monatsberichte (wo implementiert)  
- Druckbare Berichtsvorschau (wo implementiert)

#### ✅ Sync / Shared Data Folder
- Daten werden als JSON in einem gemeinsamen Ordner gespeichert  
- Sync-Logik erkennt externe Änderungen und lädt sicher neu (wo implementiert)

---

### Tech-Stack
- **.NET (WPF)**
- **C#**
- **Newtonsoft.Json**
- **Microsoft.Extensions.Logging**

---

### Projektstruktur (High-Level)
- `ManagementApp`  
  Haupt-UI für Mitarbeiter, Schichten, Abwesenheiten, Aufgaben, Berichte, Einstellungen

- `DisplayApp`  
  Anzeige-/Dashboard-Layer (Empfehlungen, Visualisierungen, Insights)

- `Shared`  
  Gemeinsame Models, Services, Utilities, JSON-Helper, Sync-Logik

---

### Datenhaltung
Teamtafel verwendet JSON-Dateien in einem konfigurierbaren Datenverzeichnis (Standard: `Data`).  
Die App kann auf einen gemeinsamen Ordner zeigen, um Multi-Geräte-Workflows zu ermöglichen.

---

### Getting Started (Deutsch)

#### Voraussetzungen
- Windows
- Visual Studio 2022+
- .NET SDK installiert (typisch `net8.0-windows`)

#### Start
1. Repository klonen:
   ```bash
   git clone https://github.com/amirmobash/Teamtafel.git
````

2. In Visual Studio öffnen
3. Startup-Projekt auswählen:

   * `ManagementApp` oder `DisplayApp`
4. Build & Run (F5)

---

## 🇬🇧 English

### Overview

**Teamtafel** is a WPF-based workforce management dashboard for planning shifts, tracking absences, managing tasks, and generating reports. Data is stored as JSON inside a shared folder, enabling simple multi-device workflows without a database.

### Highlights

#### ✅ Employee Management

* Add, edit, delete employees
* Assign roles and manage supervisors/managers
* Optional employee photo support

#### ✅ Shift Planning

* Morning/Evening/Night shifts (depending on implementation/config)
* Configurable shift capacity
* Drag & drop assignment (where implemented)
* Swap shifts and clear shift slots

#### ✅ Absence Tracking

* Vacation, sick leave, unexcused absence
* Daily absence lists grouped by category
* Prevents assigning absent employees to shifts

#### ✅ Task Management

* Create, update, delete tasks
* Status tracking: **Pending / In Progress / Completed**
* Assign employees to tasks
* Record progress and notes (where implemented)

#### ✅ Reporting

* Daily, weekly, and monthly report generation (where implemented)
* Printable report preview (where implemented)

#### ✅ Sync / Shared Data Folder

* JSON storage in a shared folder
* Sync logic detects external changes and reloads safely (where implemented)

---

### Tech Stack

* **.NET (WPF)**
* **C#**
* **Newtonsoft.Json**
* **Microsoft.Extensions.Logging**

---

### Project Structure (High-Level)

* `ManagementApp`
  Main UI and management features (employees, shifts, absences, tasks, reports, settings)

* `DisplayApp`
  Display/insight layer (recommendations, dashboards, visual summaries)

* `Shared`
  Shared models, services, utilities, JSON helpers, sync logic

---

### Data Storage

Teamtafel stores data as JSON files inside a configurable data directory (default: `Data`).
Point the app to a shared folder to support multi-device workflows.

---

### Getting Started (English)

#### Prerequisites

* Windows
* Visual Studio 2022+
* .NET SDK installed (often targeting `net8.0-windows`)

#### Run

1. Clone the repository:

   ```bash
   git clone https://github.com/amirmobash/Teamtafel.git
   ```
2. Open in Visual Studio
3. Select startup project:

   * `ManagementApp` or `DisplayApp`
4. Build & Run (F5)

---

## License

This project is provided as-is. Add your license here if needed.

---

## Tags

#Teamtafel #WPF #DotNet #CSharp #WorkforceManagement #ShiftPlanning #AbsenceTracking #TaskManagement #Reporting #JSON #SharedFolder #Dashboard #AmirMobasheraghdam

```
```
