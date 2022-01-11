using static CodiceFiscaleUtilsEN;
using static CodiceFiscaleUtilsITA;

string fiscalCode = GetFiscalCode("Mario", "Rossi", new DateTime(1950, 5, 4), 'M', "A131");
string codiceFiscale = CalcolaCodiceFiscale("Mario", "Rossi", new DateTime(1950, 5, 4), 'M', "A131");

Console.WriteLine("Fiscal Code -> " + fiscalCode);
Console.WriteLine("Codice Fiscale -> " + codiceFiscale);