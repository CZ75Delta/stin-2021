# Dokument Specifikace Požadavků
## Covid-19 Tracker - Škrbel, Novotný, Schönherr, Vican

- Verze 1.0
- Připravil Vít Škrbel
- FM TUL
- 20\. 4\. 2021

Obsah
=================
* 1 [Úvod](#1-úvod)
  * 1.1 [Účel Dokumentu](#11-účel-dokumentu)
  * 1.2 [Rozsah Programu](#12-rozsah-programu)
  * 1.3 [Glosář](#13-glosář)
  * 1.4 [Zdroje](#14-zdroje)
  * 1.5 [Přehled Dokumentu](#15-přehled-dokumentu)
* 2 [Celkový popis](#2-celkový-popis)
  * 2.1 [Prostředí Programu](#21-prostředí-programu)
  * 2.2 [Funkční Požadavky](#22-funkční-požadavky)
  * 2.3 [Charakteristiky Uživatelů](#23-charakteristiky-uživatelů)
  * 2.4 [Doplňkové Požadavky](#24-doplňkové-požadavky)
* 3 [Specifikace Požadavků](#3-specifikace-požadavků)
  * 3.1 [Externí Rozhraní](#31-externí-rozhraní)
    * 3.1.1 [Uživatelské Rozhraní](#311-uživatelské-rozhraní)
    * 3.1.2 [Softwarové Rozhraní](#312-softwarové-rozhraní)
    * 3.1.3 [Hardwarové Požadavky](#313-hardwarové-požadavky)
  * 3.2 [Funkční Požadavky](#32-funkční-požadavky)
  * 3.3 [Detailní Doplňkové Požadavky](#33-detailní-doplňkové-požadavky)
    * 3.3.1 [Bezpečnost](#331-bezpečnost)
    * 3.3.2 [Spolehlivost](#332-spolehlivost)
    * 3.3.3 [Dostupnost](#333-dostupnost)
    * 3.3.4 [Časová náročnost](#334-časová-náročnost)
    * 3.3.5 [Uzávěrka](#335-uzávěrka)

## 1. Úvod
### 1.1 Účel Dokumentu
Účelem tohoto dokumentu je představit detailní popis Covid-19 Trackeru. Vysvětlí účel a funkce programu, jeho rozhraní, podmínky za kterých musí pracovat a jak bude reagovat na externí podněty. Dokument je určen pro vývojáře a bude navrhnut zákazníkovi ke schválení.

### 1.2 Rozsah Programu
Program bude sloužit ke sledování vybraných informací o nemoci Covid-19. Bude navržen tak, aby požadované informace byly přehledně zobrazeny. Specificky bude program porovnávat data z českých a zahraničních zdrojů a zobrazovat rozdíly mezi nimi, pokud takové rozdíly existují. Tyto informace bude program ukládat do lokální relační databáze.

### 1.3 Glosář
| Pojem | Definice |
| --- | ---|
| Dokument specifikace požadavků | Dokument popisující všechny funkce navrženého programu a omezení ve kterých musí pracovat. Například tento dokument | 
| Covid-19 | Covid-19 z anglického spojení coronavirus disease 2019 je vysoce infekční onemocnění, které je způsobeno koronavirem SARS-CoV-2. První případ byl identifikován v čínském Wu-chanu v prosinci 2019. Od té doby se virus rozšířil po celém světě, což způsobilo přetrvávající pandemii.  |

### 1.4 Zdroje
IEEE 830-1998, 1998. *IEEE Recommended Practice for Software Requirements Specifications.* New York, US: The Institute of Electrical and Electronics Engineers.

### 1.5 Přehled Dokumentu
Následující kapitola, Celkový popis, tohoto dokumentu poskytuje přehled funkcionality programu. Popisuje obecné požadavky a slouží k vytvoření kontextu pro specifikaci technických požadavků v další kapitole. Třetí kapitola, Specifikace požadavků, slouží hlavně vývojářům a v technických pojmech popisuje detaily funkcionality programu. Obě části dokumentu popisují stejný program, ale jsou určeny pro jiné publikum a používají proto jiný jazyk.

## 2. Celkový popis
### 2.1 Prostředí Programu
![Prostředí programu](https://i.imgur.com/zZsgU2l.png "Prostředí programu")
*Obrázek 1 - Prostředí programu*

Program má jednoho aktéra, uživatele, který k systému přistupuje přímo. Uživatel může zvolit, která data chce zobrazit a může provést manuální aktualizaci zobrazovaných dat.

### 2.2 Funkční Požadavky
Tato sekce blíže popisuje jednotlivé případy použití programu. 

#### 2.2.1 Případ použití: **Zobrazení dat**
##### Diagram:
![Diagram Zobrazení dat](https://i.imgur.com/R78R8BD.png "Diagram Zobrazení dat")
##### Krátký popis:
Uživatel si zvolí která data chce zobrazit a upraví parametry zobrazení.
##### Popis kroků:
- Uživatel si volí která data chce zobrazit.
- Program zobrazí požadovaná data.
- Uživatel upraví parametry zobrazení.
- Program upraví zobrazení podle parametrů.
##### Reference:

#### 2.2.2 Případ použití: **Aktualizace dat**
##### Diagram:
![Diagram Aktualizace dat](https://i.imgur.com/WvbYrR0.png "Diagram Aktualizace dat")
##### Krátký popis:
Uživatel se rozhodne provést manuální aktualizaci dat.
##### Popis kroků:
- Uživatel stiskne tlačítko aktualizace dat.
- Program zkontroluje zda je k dispozici aktualizace.
  - Pokud existuje aktualizace program provede aktuazlizaci zobrazení
  - Pokud neexistuje aktualizace program upozorní uživatele.
##### Reference:

### 2.3 Charakteristiky Uživatelů
Od uživatele se očekává základní znalost použití programů v Microsoft Windows. Dále se také očekává znalost čtení grafů a použítí ovládacích prvků jako jsou tlačítka, rozbalovací nabídky apod.

### 2.4 Doplňkové Požadavky
Program poběží na počítači s operačním systémem Windows 10 a připojením k Internetu přes síť TUL. Počítač bude mít nainstalován framework .NET 5, který bude dodán společně s programem. Databázi bude spravovat program sám a není k ní nutné instalovat žádný další software.

## 3. Specifikace Požadavků
### 3.1 Externí Rozhraní
Tato sekce popisuje všechny vstupně výstupní požadavky a řešení programu.

#### 3.1.1 Uživatelské Rozhraní
Uživatelské rozhraní bude řešeno pomocí WPF. V rozhraní bude možné přepínat mezi dvěma taby pomocí prvku TabControl. Ve spodní části okna bude ProgressBar aktualizace dat, tlačítko pro manuální aktualizaci dat a tetxtovou reprezentaci aktuálního stavu programu. V pravém horním rohu bude umístěn prvek na vybrání dne, pro který se mají data zobrazovat.

První tab bude zobrazovat porovnání dat o nakažených nemocí Covid-19 v ČR. Toho dosáhne ListBoxem, ve kterém bude možné zvolit, která data se mají zobrazit k porovnání v grafu nad ním. Může se tedy zobrazit buď graf celkového počtu nakažených, či nově nakažených. Data která se budou zobrazovat v grafu nebudou záležet na vybraném datumu, zobrazí se celé období dat v databázi.
![Uživatelské rozhraní 1](https://i.imgur.com/UkWErCk.png "Uživatelské rozhraní 1")

Druhý tab zobrazuje porovnání ČR a až čtyř dalších zemí v grafu. Požadované země se dají zvolit v ListBoxu, ve kterém se dá vyhledávat pomocí vyhledávacího políčka nad ním.
![Uživatelské rozhraní 2](https://i.imgur.com/lpkEzCT.png "Uživatelské rozhraní 2")

#### 3.1.2 Softwarové Rozhraní
Aplikace bude porovnávat datové sady uložené v SQLite databázi ve složce ze které je spuštěn program. Data v této aplikaci bude získávat ze dvou zdrojů. Aplikace bude logovat chyby a informace na **[webhook Telegram bota](https://core.telegram.org/bots/api)**. Logy bude odesílat po každé aktualizaci dat, či nastalé chybě. Kontrola, zda jsou nová data pro aktualizaci bude probíhat vždy ve stanovený časový úsek v intervalech 5 minut. Identické logy se budou ukládat také do lokálního souboru ve složce aplikace v případě výpadku internetového připojení. Po obnovení připojení se odešle celý logovací soubor.

Zdroje dat:
* ##### **[MZČR Základní Přehled](https://onemocneni-aktualne.mzcr.cz/api/v2/covid-19/zakladni-prehled.json)**
  ###### Formát dat: JSON
  ![Schéma dat](https://i.imgur.com/iyTW6M1.png "Schéma dat")
  ###### Čas aktualizace: ~7:00 – 9:00 CET 
  ###### Krátký popis: Stručný náhled na základní epidemiologická data o pandemii COVID-19 v ČR. Datová sada obsahuje aktuální kumulativní počet provedených PCR a antigenních testů (včetně informace za předchozí den), potvrzené případy celkem a ve věkové skupině 65+(včetně informace za předchozí den), aktivní případy, vyléčené, úmrtí, očkování a hopitalizované pacienty.
  
* ##### **[MZČR Přehled vykázaných očkování podle krajů ČR](https://onemocneni-aktualne.mzcr.cz/api/v2/covid-19/ockovani.json)**
  ###### Formát dat: JSON
  ![Schéma dat](https://i.imgur.com/pzvLXsj.png "Schéma dat")
  ###### Čas aktualizace: ~7:00 – 9:00 CET 
  ###### Krátký popis: Datová sada poskytuje agregovaná data o vykázaných očkováních na úrovni krajů ČR. Každý řádek přehledu popisuje počet vykázaných očkování v daném dni, za věkovou skupinu, s použitím vybrané očkovací látky a ve vybraném kraji. Za jeden den tedy přehled obsahuje maximálně X řádků, kde X = počet krajů (14) x počet věkových skupin (15) x počet druhů očkovacích látek (v okamžik publikace 2) = 630. Data jsou aktualizována k času 20.00 h předchozího dne a mohou se zpětně mírně měnit z důvodu průběžného doplňování.

* ##### **[WHO Denní Přehled Nakažených a Mrtvých](https://covid19.who.int/WHO-COVID-19-global-data.csv)**
  ###### Formát dat: CSV
  ![Schéma dat](https://i.imgur.com/pVPPC6V.png "Schéma dat")
  ###### Čas aktualizace: ~0:00 – 2:00 CET 
  ###### Datová sada o připadech Covid-19 nahlášených WHO.

* ##### **[WHO Data o Vakcínách](https://covid19.who.int/who-data/vaccination-data.csv)**
  ###### Formát dat: CSV
  ![Schéma dat](https://i.imgur.com/frf38zN.png "Schéma dat")
  ###### Čas aktualizace: ~0:00 – 2:00 CET 
  ###### Datová sada o vakcinaci proti Covid-19 od WHO.

Použité nástroje a software:
* ##### **Microsoft Windows**
  ###### Verze: 19H2 (18363) - 20H2 (19042)
  ###### Zvolený operační systém nutný pro běh programu.

* ##### **.NET 5**
  ###### Verze: 5.0.5
  ###### Zvolený framework, který bude nainstalován společně s aplikací.

* ##### **C#**
  ###### Verze: 9.0
  ###### Zvolený programovací jazyk.

* ##### **[Microsoft.EntityFrameworkCore.Sqlite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite/)**
  ###### Verze: 5.0.5
  ###### Zvolený databázový systém a ORM pro ukládání dat.

* ##### **[Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/)**
  ###### Verze: 13.0.1
  ###### Nástroj pro zpracování dat ve formátu JSON.
 
* ##### **[LiveCharts](https://github.com/beto-rodriguez/LiveCharts2)**
  ###### Verze: 2
  ###### Nástroj pro zpracování dat ve formátu JSON.

#### 3.1.3 Hardwarové Požadavky
Počítač by měl být splňovat alespoň **[základní požadavky pro běh systému Windows 10](https://support.microsoft.com/cs-cz/windows/po%C5%BEadavky-na-syst%C3%A9m-windows-10-6d4e9a79-66bf-7950-467c-795cf0386715)** a být připojen k internetu.

### 3.2 Funkční Požadavky
| Případ použití  | Zobrazení dat |
| --- | --- | 
| Událost | Uživatel zvolí, která data chce zobrazit | 
| Základní postup | 1. Uživatel zvolí jeden z tabů 2. Aplikace vyhledá data pro daný tab v databázi a vykreslí je 3. Uživatel zvolí data, která chce porovnat v grafu 4. Aplikace vyhledá data a vykreslí je do grafu | 
| Alternativní postup | Pokud v 1. kroku aplikace nenajde v databázi vhodná data, načte do ní nová data ze zdrojů | 

| Případ použití  | Aktualizace dat |
| --- | --- | 
| Událost | Uživatel stiskne tlačítko pro manuální aktualizaci dat | 
| Základní postup | 1. Uživatel stiskne tlačítko manuální aktualizace 2. Aplikace stáhne data ze zdojů a porovná je s daty v databázi 3a. Pokud jsou data novější, aplikace zpracuje data a přidá je do databáze 3b. Pokud novější data nejsou k dispozici aplikace nic neudělá a pokračuje dále v činnosti | 

### 3.3 Detailní Doplňkové Požadavky
![Schéma databáze](https://i.imgur.com/ZVhsaSZ.png "Schéma databáze")
*Logické schéma dat*

#### 3.3.1 Bezpečnost
Uživatel má přístup pouze k zobrazování a aktualizace dat, není tedy možný z jeho strany útok na integritu dat. 
Přístup k datovým serverům je zařízen pomocí HTTPS dotazů. Pokud server nepředá "škodlivé" informace, nemělo by se být čeho bát.

#### 3.3.2 Spolehlivost
Aplikace musí běžet alespoň týden bez vnějšího podnětu, automaticky aktualizovat data a ta následně zobrazovat.

#### 3.3.3 Dostupnost
Aplikace musí být neustále dostupná, i v případě výpadku internetového připojení. Pokud nemůže aplikace data stáhnout musí být stále schopna zobrazovat již stažená data. Aplikace nebude připojení kontrolovat neustále, ale až poté co po manuální či automatické aktualizaci zjistí, že nemá připojení k internetu. Tato kontrola bude probíhat každých 10 minut pomocí **[této metody.](https://stackoverflow.com/a/44025507)** Po získání připojení k internetu aplikace provede aktualizaci mimo dané termíny.

#### 3.3.4 Časová náročnost
Odhad doby tvorby aplikace je zhruba 120 hodin.

#### 3.3.5 Uzávěrka
Týden před zkouškou :D
