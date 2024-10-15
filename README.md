# TapTapAvengers
Made by Yuval Willfand and Keren Schoss
# Tap Tap Revenge - Upgraded Version

### Overview:
Welcome to the upgraded version of the classic TapTap Revenge game! In this version, we've introduced a number of new features to enhance gameplay and performance. Below is an overview of the key updates:

---

### Key Features:

#### 1. **Unified Game Scene with Song Prefabs:**
- Instead of creating a separate scene for each song, we've streamlined the game by using a **single scene** for the game screen. 
- Each song is now represented by a **SongData element**. This element contains:
  - **Song Name** 
  - **Audio File** for the song 
  - **Background Image**
  - **Arrows Prefabs** for the easy and hard difficulty levels
- This design allows the game to dynamically load different songs with their corresponding settings without the need to switch between scenes, making the game more efficient and maintainable.

#### 2. **Records Saved and Displayed:**
- Players' records, including high scores and performance details for each song, are **saved to a file**.
- In the main menu, players can view these records, which are **read from the saved file**.
- This ensures that all previous gameplay data is persistently stored, even after closing and reopening the game.

#### 3. **New Difficulty Levels (Easy and Hard):**
- We've introduced a new difficulty feature. Now, each song can be played in **two versions**:
  - **Easy** (slower)
  - **Hard** (faster)
- Each difficulty level comes with its own set of **arrow prefab patterns** to match the chosen speed and complexity of the game.

---

Enjoy the rhythm and the challenge!

---
