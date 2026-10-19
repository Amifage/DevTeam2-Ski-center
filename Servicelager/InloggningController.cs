using Datalager;
using Entitetslager;
using System;
using System.Linq;

namespace Servicelager
{
    // Sköter inloggningen och håller reda på vem som är inloggad medan programmet körs
    public static class InloggningController
    {
        public static Personal? InloggadPersonal { get; private set; }

        // Returnerar true om e-post och lösenord stämmer
        public static bool LoggaIn(string epost, string lösenord)
        {
            using var unitOfWork = new UnitOfWork(new SkiContext());

            Personal? personal = unitOfWork.PersonalRepository
                .Find(p => p.Epost == epost)
                .FirstOrDefault();

            // Lösenordet jämförs här i C# så att stora/små bokstäver spelar roll
            if (personal == null || personal.Lösenord != lösenord)
            {
                return false;
            }

            InloggadPersonal = personal;
            return true;
        }

        public static void LoggaUt()
        {
            InloggadPersonal = null;
        }

        public static bool ÄrAdmin
        {
            get { return HarRoll("Administratör"); }
        }

        public static bool ÄrMarknadschef
        {
            get { return HarRoll("Marknadschef"); }
        }

        // Admin och marknadschef får sätta kredit och rabatt för företagskunder
        public static bool KanSättaKreditOchRabatt
        {
            get { return ÄrMarknadschef; }
        }

        // Bara admin får ändra inloggningar
        public static bool KanÄndraInloggningar
        {
            get { return ÄrAdmin; }
        }

        public static bool KanAndraStatus
        {
            get { return ÄrAdmin; }
        }


        private static bool HarRoll(string roll)
        {
            return InloggadPersonal != null &&
                   InloggadPersonal.Roll != null &&
                   InloggadPersonal.Roll.Contains(roll, StringComparison.OrdinalIgnoreCase);
        }
    }
}