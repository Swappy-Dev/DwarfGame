# ⛏️ Dwarven Caves

![Platform](https://img.shields.io/badge/Platform-PC%20%7C%20Windows-blue)
![Genre](https://img.shields.io/badge/Genre-Top--down%20%7C%20Rogue--like-orange)
![Keywords](https://img.shields.io/badge/Keywords-Dwarf%20%7C%20Mining%20%7C%20Dungeon%20Crawler-gold)

---

## 📝 1. Aprašymas

**Dwarven Caves** – tai 2D *Top-down* perspektyvos *Dungeon crawler / Rogue-like* žaidimas, skirtas PC platformai. 

Žaidime žaidėjas įsikūnija į nykštuko vaidmenį, kuris tyrinėja požemius, kovoja su autonomiškais priešais bei užsiima kalnakasyba. Žaidimo esmė – rinkti išteklius, strategiškai naudoti magiškus grybus suteikiant trumpalaikius efektus ir galiausiai įveikti galutinį lygio bosą.

Projektas sėkmingai išvystytas per **5 pilnus sprintus**, taikant Agile metodologiją ir užtikrinant visų užsakovo funkcinių bei nefunkcinių reikalavimų įgyvendinimą.

---

## 🖼️ 2. Vizualinė medžiaga 
*Štai keletas kadrų iš mūsų sukurto žaidimo pasaulio (UI, inventoriaus sistema ir kova su bosu):*

<img width="1363" height="764" alt="image" src="https://github.com/user-attachments/assets/687e2d67-3390-4765-91fc-3206afa8a123" />
<img width="1364" height="767" alt="image" src="https://github.com/user-attachments/assets/49c7f815-4838-4219-8d46-0e1607201bf9" />
<img width="1364" height="759" alt="image" src="https://github.com/user-attachments/assets/7c91755a-4d25-4875-91af-9ace49b0e45d" />
<img width="1361" height="758" alt="image" src="https://github.com/user-attachments/assets/5aef7f5a-04f6-4597-94e8-211813540182" />





---

## 🛠️ 3. Technologijos
*   **Žaidimo variklis / Kalba:**  Unity (C#) 
*   **Vartotojo sąsaja (UI):** Integruota žaidimo variklio GUI sistema (Canvas).
*   **Dirbtinis intelektas (AI):** Navigacijos tinkleliai (NavMesh) arba vektorių sekimo algoritmai priešų judėjimui link žaidėjo.

---

## 🎮 4. Naudotojo dokumentacija ir Valdymas

### 4.1. Sistemos reikalavimai
*   **OS:** Windows 10 / 11
*   **Valdymas:** Klaviatūra ir pelė

### 4.2. Žaidimo valdymas 
*   **Judėjimas (2D erdvėje):** `W`, `A`, `S`, `D`.
*   **Atakos ir kirtimas:** Kairysis pelės mygtukas `LMB` – kirtiklio (Pickaxe) naudojimas priešų puolimui bei žemėlapio objektu griovimui.
*   **Greitųjų mygtukų juosta (Hotbar):** Skaičiai `1`–`0`  klaviatūroje, skirti aktyvuoti grybų efektus.
*   **Šuolis/Išvengimas** Paspaudus mygtuka `Q` veikėjas pajudės link pelės žymeklio.
*   **Išsitraukti/pasidėti ginklą/kirtiklį** mygtukas `E` veikėjas rankose laikys ginklą/kirtiklį arba jį užsidės ant nugaros.
*   **Atidaromas inventorius** mygtukas `ESC` atidaro inventoriaus grafinę sąsają.
*   **Greitas daiktų perkėlimas į hotbar** laikant mygtuka `SHIFT` ir paspaudus ant grybo kairiuoju pelės mygtuku jis nukeliauja į hotbar.

### 4.3. Žaidimo mechanikos:
1.  **Mining:** Kirtikliu griaukite aukso akmenis ir rinkite iškritusią rūdą.
2.  **Survival & Buffs:** Rinkite požemyje augančius grybus:
    *   🔵 *Mėlyni grybai* – suteikia greičio efekta leidžianti žaidėjui greičiau judėti.
    *   🟢 *Žali grybai* – atstato vienos širdutės vertę žalos.
    *   🔴 *Raudoni grybai* – suteikia jėgos bonusą atakoms darydami daugiau žalos priešams.
3.  **Boss Fight:** Pasiekus lygio pabaigą, būkite pasiruošę kovai su Bosu. Saugokitės jo **3 skirtingų atakų tipų**.

---

## ⚡ 5. Kaip paleisti žaidimą
1. Atsisiųskite naujausią žaidimo versiją iš [👉 GitHub Releases puslapio](https://github.com/Swappy-Dev/DwarfGame/releases).
2. Išpakuokite atsisiųstą `.zip` archyvą.
3. Atsidarykite išpakuotą aplanką ir paleiskite: `Dwarven Caves.exe`.

---

## 🧪 6. Testavimas ir jo rezultatai (Testing)

Kiekvieno testo metu žaidimas buvo paleidžiamas iš naujo. Testavome žaidimo mechanikas, autonominį priešų AI ir UI (grafinės sąsajos) veikimą. Visų testų statusas: **Passed (Sėkmingai praėjo)**.

---

### 1 Testas: Main Menu
Šis testas tikrina, ar paleidus žaidimą matomas main menu ir, ar veikia mygtukai.

| Žingsnis | Aprašymas | Tikėtinas rezultatas |
| :--- | :--- | :--- |
| **Žingsnis 1** | Įjungiamas žaidimas ir matomas main menu ekranas su žaidimo pavadinimų ir du mygtukai (Play ir Quit) | Matomas aprašytas vaizdas. |
| **Žingsnis 2** | Paspaudus mygtuka Play su kairiuoju pelės mygtuku pasileidžia žaidimas. | Žaidimas užsikrauna ir matome pagrindinį veikėja žemėlapyje | 

---

### 2 Testas: Žaidėjo judėjimas ir kirtiklio naudojimas
Šis testas tikrina, ar nykštukas taisyklingai juda visomis kryptimis ir ar veikia kirtimo animacija/smūgis.

| Žingsnis | Aprašymas | Tikėtinas rezultatas |
| :--- | :--- | :--- |
| **Žingsnis 1** | Įjungiamas žaidimas ir paspaudžiami `W`, `A`, `S`, `D` arba rodyklių klavišai. | Nykštukas sklandžiai juda 2D erdvėje (aukštyn, žemyn, kairėn, dešinėn). | 
| **Žingsnis 2** | Nusitaikoma su pele ir paspaudžiamas kairysis pelės mygtukas. | Suveikia kirtiklio kirtimo animacija. | 

---

### 3 Testas: Aukso akmenų kirtimas (Mining) ir išteklių iškritimas
Šis testas tikrina, ar aplinkos objektai reaguoja į kirtiklį ir ar generuoja išteklius.

| Žingsnis | Aprašymas | Tikėtinas rezultatas |
| :--- | :--- | :--- |
| **Žingsnis 1** | Nykštuku prieidama prie žemėlapyje sugeneruoto aukso akmens. | Žaidėjas atsistoja kirtimo atstumu. |
| **Žingsnis 2** | Kairiuoju pelės mygtuku suduodami smūgiai į aukso akmenį. | Akmuo gauna žalą ir po kelių smūgių yra sugriaunamas. |
| **Žingsnis 3** | Akmuo visiškai sugriaunamas. | Akmens vietoje ant žemės iškrenta aukso rūda. |
| **Žingsnis 4** | Nykštuku užeinama ant iškritusios aukso rūdos. | Rūda pradingsta nuo žemės ir padidėja aukso rudos skaičius. |

---

### 4 Testas: Inventoriaus ir Hotbar sistemos veikimas
Šis testas tikrina, ar pakelti daiktai patenka į inventorių ir ar grafinė sąsaja juos teisingai atvaizduoja.

| Žingsnis | Aprašymas | Tikėtinas rezultatas |
| :--- | :--- | :--- |
| **Žingsnis 1** | Sugriaunami grybai ant žemės. | Grybai automatiškai patenka į inventorių. |
| **Žingsnis 2** | Patikrinama grafinė inventoriaus sąsaja paspaudus klaviatūros ESC mygtuka. | Pakelti daiktai atsiranda inventoriuje. |

---

### 5 Testas: Grybų rinkimas ir efektų aktyvavimas iš Hotbar
Šis testas tikrina trijų rūšių grybų (mėlynų, žalių, raudonų) panaudojimą ir jų suteikiamus efektus žaidėjui.

| Žingsnis | Aprašymas | Tikėtinas rezultatas |
| :--- | :--- | :--- |
| **Žingsnis 1** | Žemėlapyje surandami ir pakeliami mėlyni, žali bei raudoni grybai. | Grybai atsiranda inventoriaus sąsajoje. |
| **Žingsnis 2** | Iš eilės laikant `SHIFT` ir spaudžiant `LMB` grybai sudedami į Hotbar| Grybai atsiranda Hotbar juostoje. |
| **Žingsnis 3** | Paspaudžiamas klaviatūros mygtukas `1` (kur padėtas mėlynas grybas). | Grybas sunaudojamas, aktyvuojamas greičio efektas. |
| **Žingsnis 4** | Paspaudžiamas klaviatūros mygtukas `2` (žalias grybas) ir `3` (raudonas grybas). | Atstatoma gyvybe ir aktyvuojasi stiprumo efektas. |
| **Žingsnis 5** | Patikrinama žaidėjo būsena ir UI. | Gyvybiu UI aiškiai matome atstatytas gyvybes, pasikečia daroma žala ir veikėjo greitis. |

---

### 6 Testas: Autonominių priešų (AI) elgsena ir puolimas
Šis testas tikrina, ar priešai geba savarankiškai surasti žaidėją ir jį pulti.

| Žingsnis | Aprašymas | Tikėtinas rezultatas |
| :--- | :--- | :--- |
| **Žingsnis 1** | Žemėlapyje sugeneruojami priešai. | Priešai pradeda veikti autonomiškai. |
| **Žingsnis 2** | Nykštukas stovi vietoje arba juda žemėlapiu. | Priešų AI aptinka žaidėją ir pradeda judėti tiesiai link jo. |
| **Žingsnis 3** | Priešas prieina visiškai arti nykštuko. | Priešas pradeda atakos fazę. |
| **Žingsnis 4** | Priešas suduoda smūgį nykštukui. | Nykštuko gyvybiu juostoje širdutės praranda spalvą. |

---

### 7 Testas: Galutinio lygio boso (Boss) 3 atakų tipų veikimas
Šis testas tikrina boso kovos logiką ir trijų skirtingų atakų fazių pasikeitimą.

| Žingsnis | Aprašymas | Tikėtinas rezultatas |
| :--- | :--- | :--- |
| **Žingsnis 1** | Žaidėjas pasiekia lygio pabaigą ir aktyvuoja kovą su Bosu. | Bosas pabunda ir pradeda atakuoti veikėją. |
| **Žingsnis 2** | Žaidėjas kovoja su Bosu ir stebi jo elgesį. | Bosas autonomiškai naudoja 3 skirtingas atakas ir jos veikia korektiškai. |
| **Žingsnis 3** | Boso gyvybėms nukritus iki tam tikros ribos. | Bosas pradeda dažniau atakuoti žaidėją. |

---

## 👥 7. Komanda
*   **Gedvydas Daunoravičius** – Žemėlapio generacija, inventory sistema ir bosso kova.
*   **Martynas Staškūnas** – Vizualus, main menu ir pagrindinio veikėjo mechanikas.
*   **Liutauras Martinkus** – Priešu DI, valiuta, pergalės ir pralaimėjimo ekranai.

---

## 📄 8. Licencija
Šis akademinis projektas yra licencijuotas pagal **MIT** licenciją.
