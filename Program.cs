using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using GeneralTools;

namespace ProjektSemStatki {
    class Highscore {
        public string Nickname { get; set; }
        public int Wynik { get; set; }

        public Highscore(string Nickname, int Wynik) {
            this.Nickname = Nickname;
            this.Wynik = Wynik;
        }
    }

    class Pozycja // tworzy współrzędne x i y
    {
        public int x { get; set; } = -1;
        public int y { get; set; } = -1;
    }
    class Obiekt {
        Random random = new Random();
        private const int PIATKA = 5;
        private const int CZWORKA = 4;
        private const int TROJKA = 3;
        private const int DWOJKA = 2;
        private const int JEDYNKA = 1;

        public Obiekt() {

            Piatka = Generuj(PIATKA, WszystkiePozycje);
            Czworka = Generuj(CZWORKA, WszystkiePozycje);
            Trojka = Generuj(TROJKA, WszystkiePozycje);
            Dwojka = Generuj(DWOJKA, WszystkiePozycje);
            Jedynka = Generuj(JEDYNKA, WszystkiePozycje);
        }

        public int LiczbaKrokow { get; set; } = 0;

        public List<Pozycja> Piatka { get; set; }//5
        public List<Pozycja> Czworka { get; set; }//4
        public List<Pozycja> Trojka { get; set; }//3
        public List<Pozycja> Dwojka { get; set; }//2
        public List<Pozycja> Jedynka { get; set; }//1
        public List<Pozycja> Strzaly { get; set; } = new List<Pozycja>();
        public List<Pozycja> WszystkiePozycje { get; set; } = new List<Pozycja>();

        //sprawdza stan statku (wolny, zatopiony)
        public bool SprawdzPiatke { get; set; } = true;
        public bool SprawdzCzworke { get; set; } = true;
        public bool SprawdzTrojke { get; set; } = true;
        public bool SprawdzDwojke { get; set; } = true;
        public bool SprawdzJedynke { get; set; } = true;


        //sprawdza, czy któryś obiekt nie jest już zatopiony
        public bool CzyStatkiSaZniszczone { get; set; } = false;
        public bool CzyPiatkaZatopiona { get; set; } = false;
        public bool CzyCzworkaZatopiona { get; set; } = false;
        public bool CzyTrojkaZatopiona { get; set; } = false;
        public bool CzyDwojkaZatopiona { get; set; } = false;
        public bool CzyJedynkaZatopiona { get; set; } = false;

        public Obiekt SprawdzStan(List<Pozycja> PozycjeTrafione) {

            CzyPiatkaZatopiona = Piatka.Where(C => !PozycjeTrafione.Any(H => C.x == H.x && C.y == H.y)).ToList().Count == 0;
            CzyCzworkaZatopiona = Czworka.Where(B => !PozycjeTrafione.Any(H => B.x == H.x && B.y == H.y)).ToList().Count == 0;
            CzyTrojkaZatopiona = Trojka.Where(D => !PozycjeTrafione.Any(H => D.x == H.x && D.y == H.y)).ToList().Count == 0;
            CzyDwojkaZatopiona = Dwojka.Where(S => !PozycjeTrafione.Any(H => S.x == H.x && S.y == H.y)).ToList().Count == 0;
            CzyJedynkaZatopiona = Jedynka.Where(P => !PozycjeTrafione.Any(H => P.x == H.x && P.y == H.y)).ToList().Count == 0;

            //zlicza wartosci Czy***Zatopiona i zwraca wartość
            CzyStatkiSaZniszczone = CzyPiatkaZatopiona && CzyCzworkaZatopiona && CzyTrojkaZatopiona && CzyDwojkaZatopiona && CzyJedynkaZatopiona;
            return this;
        }
        public List<Pozycja> Generuj(int size, List<Pozycja> WszystkiePozycje) {
            List<Pozycja> pozycje = new List<Pozycja>();
            // v sprawdza czy istnieje już coś na tym polu
            bool CzyIstnieje = false;
            // v pętla działa tak długo, jak na danym polu nie wygenerował się żaden obiekt
            do {
                pozycje = GenerowaniePozycji(size);
                CzyIstnieje = pozycje.Where(AP => WszystkiePozycje.Exists(ShipPos => ShipPos.x == AP.x && ShipPos.y == AP.y)).Any();
            }
            while (CzyIstnieje);

            WszystkiePozycje.AddRange(pozycje);
            return pozycje;
        }
        public List<Pozycja> GenerowaniePozycji(int size) {
            List<Pozycja> pozycje = new List<Pozycja>();
            //obraca pionowo i poziomo
            int obrot = random.Next(1, size);
            int row = random.Next(1, 11);
            int column = random.Next(1, 11);
            // v modulo, żeby pozostały dwie opcje
            if (obrot % 2 != 0) {
                //kolumny od góry do dołu
                if (column - size > 0) {
                    for (int i = 0; i < size; i++) {
                        Pozycja poz = new Pozycja();
                        poz.x = row;
                        poz.y = column - i;
                        pozycje.Add(poz);
                    }
                } else {
                    for (int i = 0; i < size; i++) {
                        Pozycja poz = new Pozycja();
                        poz.x = row;
                        poz.y = column + i;
                        pozycje.Add(poz);
                    }
                }

            } else {
                // v wiersz, od lewej do prawej
                if (row - size > 0) {
                    for (int i = 0; i < size; i++) {
                        Pozycja poz = new Pozycja();
                        poz.x = row - i;
                        poz.y = column;
                        pozycje.Add(poz);
                    }
                } else {
                    for (int i = 0; i < size; i++) {
                        Pozycja poz = new Pozycja();
                        poz.x = row + i;
                        poz.y = column;
                        pozycje.Add(poz);
                    }
                }
            }
            return pozycje;
        }

        public Obiekt Strzelaj() {
            //sprawdza czy dane pole zostało trafione
            bool JuzTrafiony = false;
            Pozycja PozycjaPrzeciwnika = new Pozycja();
            //pętla działa do momentu jak pole przeciwnika jest puste, analogicznie jak z naszym
            do {
                PozycjaPrzeciwnika.x = random.Next(1, 11);
                PozycjaPrzeciwnika.y = random.Next(1, 11);
                JuzTrafiony = Strzaly.Any(EFP => EFP.x == PozycjaPrzeciwnika.x && EFP.y == PozycjaPrzeciwnika.y);
            }
            while (JuzTrafiony);
            Strzaly.Add(PozycjaPrzeciwnika);
            return this;
        }
    }
    class Program {

        static void Menu() {
            Console.WriteLine("Zagrajmy w statki! :)");
            Console.WriteLine("1. Zagraj w grę :)");
            Console.WriteLine("2. Wpisz swój nick :) ");
            Console.WriteLine("0. Opuść rozgrywkę :( ");
            int wybor = Tools.WprowadzInt($"Wybierz opcję: ", -1, 3);
            bool CzyGrac = true;
            while (CzyGrac) {
                switch (wybor) {
                    case 1: {
                            ProgramGlowny();
                            break;
                        }
                    case 2: {
                            Nick();
                            break;
                        }
                    case 0: {
                            break;
                        }
                }
            }
        }


        static string Nick() {
            Console.WriteLine("Tutaj wpisz swój nick :) :  ");
            string TwojNick = Console.ReadLine();

            Menu();
            return TwojNick;

        }

        static void Statystyki(int x, int y, Obiekt obiekt) {
            //pisze przydatne informacje dla gracza w wyznaczonym miejscu
            //pokazuje na bieżąco stan statków
            //które są okupowane, zatopione, bezpieczne
            if (x == 1 && y == 10) {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Statek:       ");
            }
            if (x == 2 && y == 10) {
                if (obiekt.CzyPiatkaZatopiona) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Piatka [5]    ");
                } else {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Piatka [5]    ");
                }
            }
            if (x == 3 && y == 10) {
                if (obiekt.CzyCzworkaZatopiona) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Czworka [4]   ");
                } else {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Czworka [4]   ");
                }
            }
            if (x == 4 && y == 10) {

                if (obiekt.CzyTrojkaZatopiona) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Trojka [3]    ");
                } else {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Trojka [3]    ");
                }
            }
            if (x == 5 && y == 10) {

                if (obiekt.CzyDwojkaZatopiona) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Dwojka [2]    ");
                } else {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Dwojka [2]    ");
                }
            }
            if (x == 6 && y == 10) {

                if (obiekt.CzyJedynkaZatopiona) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Jedynka [1]   ");
                } else {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Jedynka [1]   ");
                }

            }
            //puste pola
            if (x > 6 && y == 10) {
                for (int i = 0; i < 14; i++) {
                    Console.Write(" ");
                }
            }
        }
        static void Mapa(List<Pozycja> pozycje, Obiekt MojObiekt, Obiekt ObiektPrzeciwnika, bool PokazStatkiPrzeciwnika) {

            Oznaczenia();
            Console.WriteLine();
            if (!PokazStatkiPrzeciwnika)
                PokazStatkiPrzeciwnika = MojObiekt.CzyStatkiSaZniszczone;

            List<Pozycja> PozycjeOstrzelone = pozycje.OrderBy(o => o.x).ThenBy(n => n.y).ToList();
            List<Pozycja> PozycjeStatkow = ObiektPrzeciwnika.WszystkiePozycje.OrderBy(o => o.x).ThenBy(n => n.y).ToList();

            PozycjeStatkow = PozycjeStatkow.Where(FP => !PozycjeOstrzelone.Exists(ShipPos => ShipPos.x == FP.x && ShipPos.y == FP.y)).ToList();


            int IloscRuchow = 0;
            int IloscStatkowPrzeciwnika = 0;
            int IloscMoichStatkow = 0;
            int IloscRuchowPrzeciwnika = 0;
            // v od jakiego momentu zaczynamy pole
            char row = 'A';
            try {
                for (int x = 1; x < 11; x++) {
                    for (int y = 1; y < 11; y++) {
                        bool CzyKontynuowac = true;


                        if (y == 1) {
                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                            Console.Write("[" + row + "]");
                            row++;
                        }



                        if (PozycjeOstrzelone.Count != 0 && PozycjeOstrzelone[IloscRuchow].x == x && PozycjeOstrzelone[IloscRuchow].y == y) {

                            if (PozycjeOstrzelone.Count - 1 > IloscRuchow)
                                IloscRuchow++;

                            if (ObiektPrzeciwnika.WszystkiePozycje.Exists(ShipPos => ShipPos.x == x && ShipPos.y == y)) {

                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("[*]");

                                //Statystyki(x, y, obiekt,true);
                                CzyKontynuowac = false;
                                //continue;

                            } else {
                                Console.ForegroundColor = ConsoleColor.Black;
                                Console.Write("[X]");

                                CzyKontynuowac = false;
                                //continue;

                            }

                        }

                        if (CzyKontynuowac && PokazStatkiPrzeciwnika && PozycjeStatkow.Count != 0 && PozycjeStatkow[IloscStatkowPrzeciwnika].x == x && PozycjeStatkow[IloscStatkowPrzeciwnika].y == y) {

                            if (PozycjeStatkow.Count - 1 > IloscStatkowPrzeciwnika)
                                IloscStatkowPrzeciwnika++;

                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.Write("[O]");
                            CzyKontynuowac = false;
                        }

                        if (CzyKontynuowac) {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write("[~]");
                        }


                        Statystyki(x, y, MojObiekt);


                        if (y == 10) {
                            //przestrzeń pomiędzy planszami
                            Console.Write("     ");

                            MapaPrzeciwnika(x, row, MojObiekt, ObiektPrzeciwnika, ref IloscMoichStatkow, ref IloscRuchowPrzeciwnika);
                        }
                    }

                    Console.WriteLine();
                }

            } catch (Exception e) {
                string error = e.Message.ToString();
            }
        }

        static void MapaPrzeciwnika(int x, char row, Obiekt MojObiekt, Obiekt ObiektPrzeciwnika, ref int IloscMoichStatkow, ref int IloscRuchowPrzeciwnika) {
            List<Pozycja> PozycjePrzeciwnika = new List<Pozycja>();
            row--;
            Random random = new Random();
            List<Pozycja> PozycjeOstrzalu = ObiektPrzeciwnika.Strzaly.OrderBy(o => o.x).ThenBy(n => n.y).ToList();
            List<Pozycja> PozycjeStatkow = MojObiekt.WszystkiePozycje.OrderBy(o => o.x).ThenBy(n => n.y).ToList();

            PozycjeStatkow = PozycjeStatkow.Where(FP => !PozycjeOstrzalu.Exists(PozycjaStatku => PozycjaStatku.x == FP.x && PozycjaStatku.y == FP.y)).ToList();


            try {

                for (int y = 1; y < 11; y++) {
                    bool CzyKontynuowac = true;
                    if (y == 1) {
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.Write("[" + row + "]");
                        row++;
                    }

                    if (PozycjeOstrzalu.Count != 0 && PozycjeOstrzalu[IloscRuchowPrzeciwnika].x == x && PozycjeOstrzalu[IloscRuchowPrzeciwnika].y == y) {
                        if (PozycjeOstrzalu.Count - 1 > IloscRuchowPrzeciwnika)
                            IloscRuchowPrzeciwnika++;

                        if (MojObiekt.WszystkiePozycje.Exists(ShipPos => ShipPos.x == x && ShipPos.y == y)) {

                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write($"[*]");

                            //Statystyki(x, y, obiekt,true);
                            CzyKontynuowac = false;
                            //continue;

                        } else {
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write("[X]");

                            //Statystyki(x, y, obiekt, true);
                            CzyKontynuowac = false;
                            //continue;

                        }

                    }

                    if (CzyKontynuowac && PozycjeStatkow.Count != 0 && PozycjeStatkow[IloscMoichStatkow].x == x && PozycjeStatkow[IloscMoichStatkow].y == y) {

                        if (PozycjeStatkow.Count - 1 > IloscMoichStatkow)
                            IloscMoichStatkow++;

                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write("[O]");

                        // Statystyki(x, y, obiekt, true);
                        CzyKontynuowac = false;
                        //continue;

                    }

                    if (CzyKontynuowac) {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("[~]");
                    }


                    Statystyki(x, y, ObiektPrzeciwnika);

                }


            } catch (Exception e) {
                string error = e.Message.ToString();
            }
        }

        static Pozycja Analiza(string input, Dictionary<char, int> Koordynaty) {
            Pozycja poz = new Pozycja();

            char[] podzial = input.ToUpper().ToCharArray();

            if (podzial.Length < 2 || podzial.Length > 4) {
                return poz;
            }
            if (Koordynaty.TryGetValue(podzial[0], out int value)) {
                poz.x = value;
            } else {
                return poz;
            }

            if (podzial.Length == 3) {

                if (podzial[1] == '1' && podzial[2] == '0') {
                    poz.y = 10;
                    return poz;
                } else {
                    return poz;
                }
            }
            if (podzial[1] - '0' > 9) {
                return poz;
            } else {
                poz.y = podzial[1] - '0';
            }
            return poz;
        }

        static void Oznaczenia() {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("[ ]");
            for (int i = 1; i < 11; i++)
                Console.Write("[" + i + "]");
        }
        static Dictionary<char, int> PopulateDictionary() {
            Dictionary<char, int> koordynaty =
                     new Dictionary<char, int>
                     {
                         { 'A', 1 },
                         { 'B', 2 },
                         { 'C', 3 },
                         { 'D', 4 },
                         { 'E', 5 },
                         { 'F', 6 },
                         { 'G', 7 },
                         { 'H', 8 },
                         { 'I', 9 },
                         { 'J',10 }
                     };

            return koordynaty;
        }

        static void InfoDlaGracza(Obiekt obiekt, bool CzyStatek) {

            string title = CzyStatek ? "Twoja" : "Przeciwna";

            if (obiekt.SprawdzJedynke && obiekt.CzyJedynkaZatopiona) {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("{0} {1} jest zatopiony", title, nameof(obiekt.Jedynka));

            }
            if (obiekt.SprawdzDwojke && obiekt.CzyDwojkaZatopiona) {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("{0} {1} jest zatopiony", title, nameof(obiekt.Dwojka));
                obiekt.SprawdzDwojke = false;
            }
            if (obiekt.SprawdzTrojke && obiekt.CzyTrojkaZatopiona) {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("{0} {1} jest zatopiony", title, nameof(obiekt.Trojka));
                obiekt.SprawdzTrojke = false;
            }
            if (obiekt.SprawdzCzworke && obiekt.CzyCzworkaZatopiona) {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("{0} {1} jest zatopiona", title, nameof(obiekt.Czworka));
                obiekt.SprawdzCzworke = false;
            }

            if (obiekt.SprawdzPiatke && obiekt.CzyPiatkaZatopiona) {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("{0} {1} jest zatopiony", title, nameof(obiekt.Piatka));
                obiekt.SprawdzPiatke = false;
            }

        }
        static int ProgramGlowny() {

            Console.Clear(); // czyści konsolę, żeby nie wyświetlało się niepotrzebnie menu
            //pokazanie statków przeciwnika
            bool PokazStatkiPrzeciwnika = false;

            Obiekt MojObiekt = new Obiekt();
            Obiekt ObiektPrzeciwnika = new Obiekt();

            Dictionary<char, int> Kordy = PopulateDictionary();
            Oznaczenia();
            for (int h = 0; h < 19; h++) {
                Console.Write(" ");
            }

            Mapa(MojObiekt.Strzaly, MojObiekt, ObiektPrzeciwnika, PokazStatkiPrzeciwnika);

            int Gra;
            //100 elementów, 100 współrzędnych (A1,A2,A3 itd.)
            for (Gra = 1; Gra < 101; Gra++) {
                MojObiekt.LiczbaKrokow++;

                Pozycja pozycja = new Pozycja();

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Podaj koordynaty!:) (np. J5, B4,A1)");
                // v input gracza
                string input = Console.ReadLine();
                pozycja = Analiza(input, Kordy);
                //v jeśli wprowadzimy nieprawidłowe dane, gra poprosi nas w kółko o ponowne podanie kordów,
                // analogicznie jeśli dane pole zostało już ustrzelone
                if (pozycja.x == -1 || pozycja.y == -1) {
                    Console.WriteLine("Nieprawidłowe dane!");
                    Gra--;
                    continue;
                }

                if (MojObiekt.Strzaly.Any(EFP => EFP.x == pozycja.x && EFP.y == pozycja.y)) {
                    Console.WriteLine("Już strzelono w te kordynaty!");
                    Gra--;
                    continue;
                }
                ObiektPrzeciwnika.Strzelaj();
                var indeks = MojObiekt.Strzaly.FindIndex(p => p.x == pozycja.x && p.y == pozycja.y);

                if (indeks == -1)
                    MojObiekt.Strzaly.Add(pozycja);

                Console.Clear();
                MojObiekt.WszystkiePozycje.OrderBy(o => o.x).ThenBy(n => n.y).ToList();
                MojObiekt.SprawdzStan(ObiektPrzeciwnika.Strzaly);

                ObiektPrzeciwnika.WszystkiePozycje.OrderBy(o => o.x).ThenBy(n => n.y).ToList();
                ObiektPrzeciwnika.SprawdzStan(MojObiekt.Strzaly);

                Oznaczenia();
                for (int h = 0; h < 19; h++) {
                    Console.Write(" ");
                }
                Mapa(MojObiekt.Strzaly, MojObiekt, ObiektPrzeciwnika, PokazStatkiPrzeciwnika);
                // pokazujemy nasze statki, nie przeciwnika
                InfoDlaGracza(MojObiekt, true);
                InfoDlaGracza(ObiektPrzeciwnika, false);
                //v jeśli wszystkie statki są zniszczone, wychodzimy z pętli, koniec gry
                if (ObiektPrzeciwnika.CzyStatkiSaZniszczone || MojObiekt.CzyStatkiSaZniszczone) {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    KoniecGry(ObiektPrzeciwnika, MojObiekt, Gra);
                    break;
                }

            }
            // ! - bool
            return Gra;
        }

        public static void KoniecGry(Obiekt ObiektPrzeciwnika, Obiekt MojObiekt, int Gra) {
            if (ObiektPrzeciwnika.CzyStatkiSaZniszczone && !MojObiekt.CzyStatkiSaZniszczone) {
                Console.WriteLine("Koniec gry, wygrałeś.");
                Console.WriteLine("Twój wynik: {0} ", Gra);
                Console.WriteLine("Podaj nick: ");
                string playerName = Console.ReadLine();

                AktualizujPlikHighscores(Gra, playerName); // Zapisz dane skończonej gry do pliku

            } else if (!ObiektPrzeciwnika.CzyStatkiSaZniszczone && MojObiekt.CzyStatkiSaZniszczone) {
                Console.WriteLine("Koniec gry, przegrałeś.");

            } else {
                Console.WriteLine("Koniec gry, remis.");
            }

            Console.WriteLine("Czy chcesz zagrać jeszcze raz? Y/N");
            Console.ReadKey();
            // Tutaj dodaj logikę resetująca gre / wyjście z programu
            return;
        }

        public static void AktualizujPlikHighscores(int currentScore, string playerName) {

            // highscores.txt
            SprawdzPlikHighscore();

            // Stwórz pustą tablice highscore'ów
            string[] existingHighScoresArray = File.ReadAllLines("highscores.txt"); // Wczytaj obecną zawartosc pliku highscores.txt do tablicy
            List<string> highScoresList = new List<string>(existingHighScoresArray); // Konwersja tablicy na listę
            string currentGameData = playerName + ": " + currentScore.ToString(); // Dane obecnej gry

            highScoresList.Add(currentGameData); // Dodaje dane obecnej gry na koniec listy

            File.WriteAllLines("highscores.txt", highScoresList); // Nadpisz plik
        }

        public static void SprawdzPlikHighscore() {

            // Sprawdz czy plik highscores.txt istnieje i stwórz go, jeśli nie
            if (!File.Exists("highscores.txt")) {
                File.Create("highscores.txt");
            }
        }

        public static void WyswietlPosortowaneHighscores() {

            string[] highScores = File.ReadAllLines("highscores.txt");

            highScores = highScores.OrderByDescending(x => int.Parse(x.Split(':')[1])).Reverse().ToArray();

            foreach (var item in highScores) {
                Console.WriteLine(item);
            }
        }
        class Score {
            public string Name { get; set; }
            public int Value = ProgramGlowny();
        }
        static void Main(string[] args) {
            Menu();

            // Testowanie high score'ow
            /*
            Obiekt MojeStatki = new Obiekt();
            Obiekt JegoStatki = new Obiekt();
            JegoStatki.CzyStatkiSaZniszczone = true;
            KoniecGry(JegoStatki, MojeStatki, 32);
            */

            // WyswietlPosortowaneHighscores();
        }
    }
}