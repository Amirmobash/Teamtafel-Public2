# Teamtafel-Public

Teamtafel is a WPF-based workforce management dashboard for planning shifts, tracking absences, managing tasks, and generating reports 

---

## Highlights

- **Employee Management**
  - Add, edit, delete employees
  - Assign roles and manage supervisors/managers
  - Optional employee photo support

- **Shift Planning**
  - Morning/Evening shifts
  - Configurable shift capacity
  - Drag & drop assignment (where implemented)
  - Swap shifts and clear shift slots

- **Absence Tracking**
  - Vacation, sick leave, unexcused absence
  - Daily absence lists grouped by category
  - Prevents assigning absent employees to shifts

- **Task Management**
  - Create, update, delete tasks
  - Task status tracking (Pending / In Progress / Completed)
  - Assign employees to tasks
  - Record progress and notes (where implemented)

- **Reporting**
  - Daily, weekly, and monthly report generation (where implemented)
  - Printable report preview (where implemented)

- **Sync / Shared Data Folder**
  - Data stored as JSON in a shared folder
  - Sync manager detects external changes and reloads safely (where implemented)

---

## Tech Stack

- **.NET (WPF)**
- **C#**
- **Newtonsoft.Json**
- **Microsoft.Extensions.Logging**

---

## Project Structure (High-Level)

- `ManagementApp`  
  Main UI and management features (employees, shifts, absences, tasks, reports, settings)

- `DisplayApp`  
  Display/insight layer (AI recommendations, dashboards, visual summaries)

- `Shared`  
  Shared models, services, utilities, JSON helpers, date helpers, sync logic

---

## Data Storage

Teamtafel uses JSON files stored in a configurable data directory (default: `Data`).  
The app can be pointed to a shared folder to support multi-device workflows.

---

## Getting Started

### Prerequisites
- Windows
- Visual Studio 2022+
- .NET SDK installed (the project may target `net8.0-windows`)

### Run
1. Clone the repository:
   ```bash
   git clone https://github.com/amirmobash/Teamtafel.git     Amir Mobasheraghdam
