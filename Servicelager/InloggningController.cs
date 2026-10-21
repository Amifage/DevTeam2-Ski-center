using Datalager;
using Entitetslager;
using System;
using System.Linq;

namespace Servicelager
{
    public class InloggningController
    {
        public static Personal? InloggadPersonal { get; private set; }

        public static bool LoggaIn(string epost, string lösenord)
        {
            using var unitOfWork = new UnitOfWork(new SkiContext());

            Personal? personal = unitOfWork.PersonalRepository
                .Find(p => p.Epost == epost)
                .FirstOrDefault();

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

        public static bool ArAdmin
        {
            get { return HarRoll("Administratör"); }
        }

        public static bool ArMarknadschef
        {
            get { return HarRoll("Marknadschef"); }
        }

        public static bool KanSattaKreditOchRabatt
        {
            get { return ArMarknadschef; }
        }

        public static bool KanAndraInloggningar
        {
            get { return ArAdmin; }
        }

        public static bool KanAndraStatus
        {
            get { return ArAdmin; }
        }

        private static bool HarRoll(string roll)
        {
            return InloggadPersonal != null &&
                   InloggadPersonal.Roll != null &&
                   InloggadPersonal.Roll.Contains(roll, StringComparison.OrdinalIgnoreCase);
        }
    }
}