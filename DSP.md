# Dokument Specifikace Požadavků
## Covid-19 Tracker - Škrbel, Novotný, Schönherr, Vican

- Verze 0.1 
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
  * 2.3 [Charekteristiky Uživatelů](#23-charekteristiky-uživatelů)
  * 2.4 [Doplňkové Požadavky](#24-doplňkové-požadavky)
* 3 [Specifikace Požadavků](#3-specifikace-požadavků)
  * 3.1 [Externí Rozhraní](#31-externí-rozhraní)
    * 3.1.1 [Uživatelské Rozhraní](#311-uživatelské-rozhraní)
    * 3.1.2 [Hardwarové Rozhraní](#312-hardwarové-rozhraní)
    * 3.1.3 [Softwarové Rozhraní](#313-softwarové-rozhraní)
  * 3.2 [Funkční Požadavky](#32-funkční-požadavky)
  * 3.3 [Detailní Doplňkové Požadavky](#33-detailní-doplňkové-požadavky)
    * 3.3.2 [Bezpečnost](#332-bezpečnost)
    * 3.3.3 [Spolehlivost](#333-spolehlivost)
    * 3.3.4 [Dostupnost](#334-dostupnost)
    * 3.3.5 [Instalace](#335-instalace)
    * 3.3.6 [Distribuce](#336-distribuce)
    * 3.3.7 [Uzávěrka](#337-uzávěrka)

## 1. Úvod
### 1.1 Účel Dokumentu
Účelem tohoto dokumentu je představit detailní popis Covid-19 Trackeru. Vysvětlí účel a funkce programu, jeho rozhraní, podmínky za kterých musí pracovat a jak bude reagovat na externí podněty. Dokument je určen pro vývojáře a bude navrhnut zákazníkovi ke schválení.

### 1.2 Rozsah Programu
Program bude sloužit ke sledování vybraných informací o nemoci Covid-19. Bude navržen tak, aby požadované informace byly přehledně zobrazeny. Specificky bude program porovnávat data z českých a zahraničních zdrojů a zobrazovat rozdíly mezi nimi, pokud takové rozdíly existují. Tyto informace bude program ukládat do lokální relační databáze.

### 1.3 Glosář
| Pojem            | Definice         |
| -------------    |:-------------:   |
|                  |                  | 
|                  |                  |
|                  |                  |

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
1. Uživatel si volí která data chce zobrazit.
2. Program zobrazí požadovaná data.
3. Uživatel upraví parametry zobrazení.
4. Program upraví zobrazení podle parametrů.
##### Reference:

#### 2.2.2 Případ použití: **Aktualizace dat**
##### Diagram:
![Diagram Aktualizace dat](https://i.imgur.com/WvbYrR0.png "Diagram Aktualizace dat")
##### Krátký popis:
Uživatel se rozhodne provést manuální aktualizaci dat.
##### Popis kroků:
1. Uživatel stiskne tlačítko aktualizace dat.
2. Program zkontroluje zda je k dispozici aktualizace.
2a. Pokud existuje aktualizace program provede aktuazlizaci zobrazení
2b. Pokud neexistuje aktualizace program upozorní uživatele.
##### Reference:

### 2.3 Charekteristiky Uživatelů
Od uživatele se očekává základní znalost použití programů v Microsoft Windows. Dále se také očekává znalost čtení grafů a použítí ovládacích prvků jako jsou tlačítka, rozbalovací nabídky apod.

### 2.4 Doplňkové Požadavky
Program poběží na počítači s operačním systémem Windows 10 a připojením k Internetu přes síť TUL. Počítač bude mít nainstalován framework .NET 5, který bude dodán společně s programem. Databázi bude spravovat program sám a není k ní nutné instalovat žádný další software.

## 3. Specifikace Požadavků
### 3.1 Externí Rozhraní
Tato sekce popisuje všechny vstupně výstupní požadavky a řešení programu.

#### 3.1.1 Uživatelské Rozhraní
Uživatelské rozhraní bude řešeno pomocí WPF. V rozhraní bude možné přepínat mezi dvěma taby pomocí prvku TabControl. Ve spodní části okna bude ProgressBar aktualizace dat, tlačítko pro manuální aktualizaci dat a tetxtovou reprezentaci aktuálního stavu programu.

První tab bude zobrazovat porovnání dat o nakažených nemocí Covid-19 v ČR. Toho dosáhne ListBoxem, ve kterém bude možné zvolit, která data se mají zobrazit k porovnání v grafu nad ním.
![Uživatelské rozhraní 1](https://i.imgur.com/DHCs1Od.png "Uživatelské rozhraní 1")

Druhý tab zobrazuje porovnání ČR a až čtyř dalších zemí v grafu. Požadované země se dají zvolit v ListBoxu, ve kterém se dá vyhledávat pomocí vyhledávacího políčka nad ním.
![Uživatelské rozhraní 2](https://i.imgur.com/KhyOE68.png "Uživatelské rozhraní 2")

#### 3.1.2 Softwarové Rozhraní
Použité nástroje:
* ##### Název: **Microsoft Windows**
  ###### Verze: 19H2 (18363) - 20H2 (19042)
  ###### Krátký popis: Zvolený operační systém nutný pro běh programu.

* ##### Název: **.NET 5**
  ###### Verze: 5.0.5
  ###### Krátký popis: Zvolený framework, který bude nainstalován společně s aplikací.

* ##### Název: **C#**
  ###### Verze: 9.0
  ###### Krátký popis: Zvolený programovací jazyk.

* ##### Název: **[Microsoft.EntityFrameworkCore.Sqlite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite/)**
  ###### Verze: 5.0.5
  ###### Krátký popis: Zvolený databázový systém a ORM pro ukládání dat.

* ##### Název: **[Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/)**
  ###### Verze: 13.0.1
  ###### Krátký popis: Nástroj pro zpracování dat ve formátu JSON.
 
* ##### Název: **[LiveCharts](https://github.com/beto-rodriguez/LiveCharts2)**
  ###### Verze: 2
  ###### Krátký popis: Nástroj pro zpracování dat ve formátu JSON.

#### 3.1.3 Hardwarové Požadavky
Počítač by měl být splňovat alespoň základní požadavky pro běh systému Windows 10 a být připojen k internetu.

### 3.2 Funkční Požadavky

### 3.3 Detailní Doplňkové Požadavky

#### 3.3.1 Bezpečnost
Uživatel má přístup pouze k zobrazování a aktualizace dat, není tedy možný z jeho strany útok na integritu dat. 
Přístup k datovým serverům je zařízen pomocí HTTPS dotazů. Pokud server nepředá "škodlivé" informace, nemělo by se být čeho bát.

#### 3.3.2 Spolehlivost
Aplikace musí běžet alespoň týden bez vnějšího podnětu, automaticky aktualizovat data a ta následně zobrazovat.

#### 3.3.3 Dostupnost
Aplikace musí být neustále dostupná, i v případě výpadku internetového připojení. Pokud nemůže aplikace data stáhnout musí být stále schopna zobrazovat již stažená data.
