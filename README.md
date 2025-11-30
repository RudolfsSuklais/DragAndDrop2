# DragAndDropGames

Unity 2D Drag And Drop games for children +6

\*\*To do list:

-   [x] Create the necessary folders
-   [x] Add necessary assets
-   [x] Add cars on the map
-   [x] Create C# script for drag and drop
-   [x] Create C# script for transformation
-   [x] Create C# script for object fixation
-   [x] Add necessary sounds and audio sources
-   [x] Create logic for winning
-   [x] Create camera script for zoom-in/out and camera restrictions
-   [x] Add winning screen
-   [x] Add more sounds


# 🚗 DragAndDropGames

**Unity 2D Drag and Drop spēle bērniem (6+)**

Projekts izstrādāts kā programmēšanas uzdevums darbam ar objektiem **C# valodā** un **Unity 2D dzinī**.  
Spēles mērķis ir attīstīt loģisko domāšanu, uzmanību un precizitāti, velkot un novietojot transportlīdzekļus pareizajās vietās pilsētas kartē.

---

## 🕹️ Spēles apraksts

### 🎮 Velc un Nomet (Level 1 & 2)
Spēlētājam ir jānovieto dažādi transportlīdzekļi (auto, vilciens, lidmašīna u.c.) pareizajās vietās uz pilsētas kartes.  
Katru reizi, kad spēle tiek palaista, transportlīdzekļu pozīcijas tiek **ģenerētas nejauši**, tādēļ katra spēle ir unikāla.

**Galvenās iespējas:**
- Drag & Drop funkcionalitāte
- Objekta rotācija un izmēra maiņa
- Objekta “fiksācija” pareizajā vietā
- Animācijas un skaņas efekti, novietojot objektu
- Laika skaitītājs un zvaigžņu vērtējums (no 0 līdz 3)
- Uzvaras ekrāns ar rezultātu
- Iespēja restartēt spēli vai atgriezties sākuma izvēlnē

---

## 🏙️ Projekta struktūra

### Scēnas:
1. **Main Menu Scene**
   - Trīs pogas: “Iziet”, “Game 1”, “Car Game”
   - Skaņas efekti un animācijas
   - Scēnu pārslēgšana un programmas aizvēršana

2. **City Scene**
   - Pilsētas karte ar transportlīdzekļiem
   - Objekti rotē un tiek novietoti atbilstoši
   - Skaņas efekti un animācijas katram objektam
   - Uzvaras loģika ar taimeri un zvaigžņu sistēmu



## 🔧 Izmantotās tehnoloģijas

- **Unity 2022+** (2D Template)
- **C# (MonoBehaviour skripti)**
- **Git + GitHub (versiju kontrole)**
- **Audio & Animation komponentes**
- **UI elementi (Canvas, Buttons, Images, Text)**


---

## 🏆 Spēles beigas nosacījumi

- Visi transportlīdzekļi ir pareizajās vietās vai iznīcināti.
- Tiek parādīts rezultāts ar:
  - Kopējo spēles laiku (hh:mm:ss)
  - Iegūto zvaigžņu skaitu

---

## 📸 Ekrānuzņēmumi

### 🏁 Galvenā izvēlne
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/47275bd2-baa1-47a1-85f7-dea726b12ac4" />


### 🚗 Spēles aina
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/97ef1aab-9900-4b1d-ac86-dff08f38bdb8" />


### ⭐ Spēles beigu ekrāns
<img width="1920" height="1080" alt="Untitled design (11)" src="https://github.com/user-attachments/assets/f29d6455-f3d6-4a62-9b42-b0672d3d82a0" />



---

## 📦 Build versijas

| Platforma | Fails | Atrašanās vieta |
|------------|--------|----------------|
| **Windows (.exe)** | `DragAndDropGames.exe` | [Releases → Windows Build](../../releases) |


---
# 🎮 Hanojas Torņa Spēle — Unity

Šī ir pilnībā funkcionējoša *Hanojas torņa* spēle, kas izveidota, izmantojot Unity UI sistēmu.  
Spēlētāja uzdevums ir pārvietot visus diskus no A torņa uz B vai C torni, ievērojot klasiskos noteikumus:

1. Vienā gājienā drīkst pārvietot tikai vienu disku  
2. Drīkst pārvietot tikai torņa augšējo disku  
3. Lielāku disku nedrīkst likt uz mazāka diska  

Spēle ietver vilkšanas-novilšanas vadību, taimeri, soļu skaitītāju, zvaigžņu sistēmu, uzvaras paneli un pēc izvēles arī Rewarded Ads reklāmas.

---

## 📁 Scēnas struktūra

### **Galvenie objekti**
| Objekts | Apraksts |
|---------|----------|
| **GameManager** | Kontrolē spēles loģiku: gājienus, taimeri, zvaigznes, disku izveidi, uzvaras noteikumus. |
| **HanoiTower (A/B/C)** | Glabā disku krāvumu, aprēķina disku pozīcijas un validācijas. |
| **DiskPrefab** | UI disks, kas tiek ģenerēts spēles sākumā. |
| **DiskDragHandler** | Atbild par diska pārvietošanu, novietošanu un animāciju. |
| **WinPanel** | Uzvaras panelis ar statistiku, restartēšanas un iziešanas pogām. |

---

## 🧠 GameManager funkcijas

### ✔ Diski tiek izveidoti automātiski
Sākot spēli, visi diski tiek ģenerēti A tornī:

Zvaigžņu sistēma

Balstīta uz spēles pabeigšanas laiku:

Laiks	Zvaigznes
≤ 60s	⭐⭐⭐
≤ 80s	⭐⭐
≤ 120s	⭐

<img width="1100" height="538" alt="image" src="https://github.com/user-attachments/assets/cfbe1051-ab91-405b-a60c-75a6b70a593f" />
<img width="1076" height="534" alt="image" src="https://github.com/user-attachments/assets/dd43d970-e3ca-4916-8eea-24321dd51683" />





## 👨‍💻 Autors

**Rūdolfs Šuklais**  
💾 Izstrādes vide: *Unity 2D, Visual Studio Code, GitHub*  
📅 Gads: 2025

