# Civil Service

![Unity](https://img.shields.io/badge/Unity-100000?style=for-the-badge&logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)

> "Welcome, Processor. Your task is simple: Fill the quota."

**Civil Service** is a dystopian simulation game built in Unity. You play as an intake officer for a walled City, tasked with processing a never-ending line of refugees. Balancing empathy against survival, you must enforce strict, ever-changing protocols to earn your daily wage and avoid starvation.

## About The Game

Heavily inspired by *Papers, Please*, this game challenges players' attention to detail and ability to work under pressure. As the days progress, "Management" introduces new tools and stricter rules. One mistake doesn't just mean a reprimand—it cuts into the meager funds you need to survive.

### Key Features
* **7-Day Campaign:** A scripted difficulty curve that introduces new mechanics daily.
* **Procedural Generation:** Every refugee is randomised using a sprite library, ensuring no two shifts are the same.
* **Interactive Desk Tools:**
    * **Shutter System:** Manually control the flow of the day.
    * **Magnifying Glass:** Inspect masks for cracks.
    * **ID Scanner:** Verify barcode authenticity and detect smudges.
* **Economy & Survival:** Manage your "Merits." If you run out of money for food, you are terminated.
* **Reactive Narrative:** "Management" watches your every move, offering different feedback based on your performance.

## How to Play

1.  **Start the Shift:** Read the **Rulebook** for the day's protocols, then click **OPEN** on the shutter button.
2.  **Process Applicants:** Click "Call Next" to bring in a refugee.
3.  **Inspect:** Use your eyes and tools to check for defects based on the current day's rules.
    * *Day 1:* Admit everyone.
    * *Day 2:* Reject **Bloody** masks.
    * *Day 3:* Reject **Cracked** masks (Use Magnifier).
    * *Day 4:* Reject **Smudged/Missing** barcodes (Use Scanner).
4.  **Decide:** Click the **Green Button** to Approve or **Red Button** to Reject.
5.  **Survive:** Complete the quota before 4:00 PM. High accuracy earns bonuses; mistakes cost you money.

**Controls:**
* **Left Click:** Interact with UI; pick up tools.
* **Right Click:** Drop the currently held tool (Scanner.

## Technical Implementation

* **Engine:** Unity 2D (C#) (6000.3.2f1)

## Installation

1.  Clone the repository:
    ```bash
    git clone [https://github.com/lilnoggi/Game_Jammy.git](https://github.com/lilnoggi/Game_Jammy.git)
    ```
2.  Open **Unity Hub**.
3.  Click **Add** and select the cloned folder.
4.  Open the project (Recommended version: Unity 6000.3.2f1).
5.  Open the scene located in `Assets/Scenes/MainMenu`.
6.  Press **Play**.

## Credits

* **Design & Development:** Florin, Campbell, Mani
* **Art:** Florin, Campbell, Mani
* **Audio:** Florin, Mani

---
*Status: Vertical Slice*
