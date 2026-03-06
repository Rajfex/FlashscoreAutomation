# Flashscore Automation
FlashscoreAutomation is a C# console application that automates the process of collecting football league standings from Flashscore and enriching them with real‑time temperature data based on each league’s geographic location. The combined results are exported to an Excel file using EPPlus.

## Installation and Setup
Clone the repository:
``` bash
git clone https://github.com/Rajfex/FlashscoreAutomation.git
```
Open the solution file ```(.sln)``` in Visual Studio.

Build the project, then run the application.

A sample ```leagues.json``` file is included in the project so you can run the application immediately.
After building the project, place this file in:
```
bin\Debug\net10.0\
```
The generated Excel file will be saved automatically to your Documents folder.
