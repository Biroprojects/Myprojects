Ez a projekt egy Kanban board alkalmazás, amely tartalmazza a drag and drop funkciót, és lokális adatbázisból fut. Az adatbázis kezeléséhez XAMPP-ot használtam, amelyben az Apache és a MySQL szolgáltatások futnak. Az alkalmazás Visual Studio-ban készült ASP.NET Core Web App(Razor Pagesben), és az Entity Framework Code-First megközelítést használtam az adatbázis létrehozásához.

Fő funkciók: 
-Felhasználók regisztrációja és belépése
-Admin és normál felhasználói jogosultságok külön kezelése
-Címkék kezelése (hozzáadás és kiválasztás az adatbázisból)
-Drag and drop feladatkezelés
-Adatok exportálása XML formátumba
-különböző diagrammok
-Boardok, listák, taskok módosítása (CRUD funkciók)

Telepítési Útmutató
1. XAMPP telepítése és beállítása
Töltsd le a XAMPP telepítőt. link: https://www.apachefriends.org/hu/download.html
Telepítés: Telepítsd rendszergazdaként a következő lépésekkel: Next → Next → Finish.
Szolgáltatások elindítása:
Indítsd el az Apache és a MySQL szolgáltatásokat a XAMPP vezérlőpulton.
Ha valamelyik szolgáltatás nem indul el, valószínűleg az alapértelmezett port (80-as) foglalt:
Apache Port Konfiguráció:
Config → Apache (httpd.conf) → Keresd meg a Listen 80 sort → Módosítsd egy szabad portra, pl. Listen 8080.

2. Adatbázis létrehozása
Nyisd meg a böngészőt, és lépj be a phpMyAdmin felületre:
Alapértelmezett cím: http://localhost/phpmyadmin
Ha módosítottad a portot: http://localhost:<módosított port>/phpmyadmin
Válaszd a Felhasználói fiókok menüpontot → Új felhasználói fiók hozzáadása.
Töltsd ki a következő adatokat:
Felhasználónév: Tetszőleges név
Jelszó: Tetszőleges jelszó
Pipáld be az alábbi opciókat:
Azonos nevű adatbázis létrehozása, és az összes jog engedélyezése
Az összes jog engedélyezése karakterhelyettesítős néven (username_%).
globális jogok összes bejelölése
Görgess le, majd kattints az Indítás gombra.

3. Kapcsolat beállítása az appsettings.json fájlban
A projekt gyökérkönyvtárában keresd meg a appsettings.json fájlt.
Győződj meg róla, hogy a kapcsolati string megfelel az alábbi formátumnak:
vagyis mikor létrehoztad az adatbázist a gépnév nek localhost ot kell oda írni az a felhasználónévnek és a jelszónak a kanbanboard-ot vagy mást is lehet viszont akkor át kell írni a ConnectionStringet a visual studion belül.
"ConnectionStrings": {
  "KanbanContextConnection": "Server=localhost;Database=kanbanboard;User=kanbanboard;Password=kanbanboard"
}

4. Címkék hozzáadása az adatbázishoz
A phpMyAdmin felületén válaszd ki a létrehozott adatbázist, majd keresd meg a Label táblát.
Kattints a Beszúrás menüpontra.
Add meg a következő adatokat:
ID: Egyedi azonosító (pl. 1, 2, 3...)
Name: A címke neve (pl. Urgent, Important, Medium, Low)
Color: A címke színe hexadecimális formában (pl. #FF0000).
Példák:

ID	Name	Color
1	Urgent	#FF0000
2	Important	#FF00FF
3	Medium	#00FF00
4	Low	#0000FF

5. Adatbázis migrációk futtatása
Nyisd meg a Visual Studio-ban a projektet.
Nyisd meg a Package Manager Console-t (Tools → NuGet Package Manager → Package Manager Console).
Futtasd a következő parancsot az adatbázis létrehozásához: (a migrációk már létrejöttek ezért csak updatelni kell az adatbázist)
Update-Database

6. Adminisztrátor fiók létrehozása
Az alkalmazás indításakor automatikusan létrejön egy adminisztrátor fiók a következő adatokkal:
Email: admin@example.com
Jelszó: Minda1

Köszönöm ha kipróbáltad a projektemet!:)

Bármiféle kérdés esetén a következő emailen elérhető vagyok:
brokers0927@gmail.com
