/// <summary>
/// A set of generic utils to retrieve, calculate and check the Fiscal Code for an italian citizen.
/// ref.: <a href="https://www.ryadel.com/en/italian-tax-code-fiscal-code-vat-id-c-sharp-class/">https://www.ryadel.com/en/italian-tax-code-fiscal-code-vat-id-c-sharp-class/</a>
/// </summary>
public static class CodiceFiscaleUtilsEN
{
    #region Private Members
    private static readonly string Months = "ABCDEHLMPRST";
    private static readonly string Vocals = "AEIOU";
    private static readonly string Consonants = "BCDFGHJKLMNPQRSTVWXYZ";
    private static readonly string OmocodeChars = "LMNPQRSTUV";
    private static readonly int[] ControlCodeArray = new[] { 1, 0, 5, 7, 9, 13, 15, 17, 19, 21, 2, 4, 18, 20, 11, 3, 6, 8, 12, 14, 16, 10, 22, 25, 24, 23 };
    private static readonly Regex CheckRegex = new Regex(@"^[A-Z]{6}[\d]{2}[A-Z][\d]{2}[A-Z][\d]{3}[A-Z]$");
    #endregion Private Members

    #region Public Methods
    /// <summary>
    /// Retrieves a formally correct Fiscal Code given the required fields:
    /// 
    /// - FirstName
    /// - LastName
    /// - Date of Birth
    /// - Gender
    /// - ISTAT code
    /// 
    /// NOTES: 
    /// 
    /// - The ISTAT code depends to the citizen's birth location and can be retrieved from the following lists:
    ///   http://www.agenziaentrate.gov.it/wps/content/Nsilib/Nsi/Strumenti/Codici+attivita+e+tributo/Codici+territorio/Comuni+italia+esteri/
    ///   
    /// - There are no guarrantees that the returned Fiscal Code is the correct one for that citizen, 
    ///   as this function does not take edge case scenarios into account. For more info, look for "omocodia" here:
    ///   http://www.agenziaentrate.gov.it/wps/content/Nsilib/Nsi/Home/CosaDeviFare/Richiedere/Codice+fiscale+e+tessera+sanitaria/Richiesta+TS_CF/SchedaI/FAQ+sul+Codice+Fiscale/
    /// </summary>
    /// <param name="name">First Name</param>
    /// <param name="lastName">Last Name</param>
    /// <param name="birthDate">Date of Birth</param>
    /// <param name="gender">Gender ('M' or 'F')</param>
    /// <param name="istatCode">ISTAT code (1 letter, 3 numbers. Es.: H501 for Rome, Italy)</param>
    /// <returns>A valid Fiscal Code, compatible with all the given input fields</returns>
    public static string GetFiscalCode(string firstName, string lastName, DateTime birthDate, char gender, string istatCode)
    {
        if (String.IsNullOrEmpty(firstName)) throw new NotSupportedException("ERROR: First Name is required.");
        if (String.IsNullOrEmpty(lastName)) throw new NotSupportedException("ERROR: Last Name is required.");
        if (gender != 'M' && gender != 'F') throw new NotSupportedException("ERROR: Gender must either be 'M' or 'F'.");
        if (String.IsNullOrEmpty(istatCode)) throw new NotSupportedException("ERROR: ISTAT code is required.");

        //string fc = GetLastNameCode(lastName) + GetNameCode(name) + GetBirthDateByGenderCode(birthDate, gender);
        string fc = String.Format("{0}{1}{2}{3}",
                                        GetLastNameCode(lastName),
                                        GetFirstNameCode(firstName),
                                        GetBirthDateByGenderCode(birthDate, gender),
                                        istatCode
                                    );
        fc += GetControlChar(fc);
        return fc;
    }

    /// <summary>
    /// Checks for a Fiscal Code formal integrity using the following criteria:
    /// 
    /// - check for null/empty value
    /// - check for standard Regex [A-Z]{6}[\d]{2}[A-Z][\d]{2}[A-Z][\d]{3}[A-Z]
    /// - check for a valid Control Character
    /// 
    /// NOTES:
    /// - Even if this function validates a Fiscal Code as OK, there are no guarrantees that the Fiscal Code is valid and existing.
    ///   The only way to check this is to use the official tools provided by "Agenzia delle Entrate", such as:
    ///   https://telematici.agenziaentrate.gov.it/VerificaCF/Scegli.do?parameter=verificaCf
    ///   Or other services that are connected to the "Agenzia delle Entrate" database and thus able to perform lookups there.
    /// </summary>
    /// <param name="fc">The Fiscal Code to check</param>
    /// <returns>TRUE if the fiscal code is correct, FALSE otherwise</returns>
    public static bool IsValid(string fc)
    {
        if (String.IsNullOrEmpty(fc) || fc.Length < 16) return false;
        fc = Normalize(fc, false);
        if (!CheckRegex.Match(fc).Success)
        {
            // Regex failed: it can be either an omocode or an invalid Fiscal Code
            string nonOmocodeFC = ReplaceOmocodeChars(fc);
            if (!CheckRegex.Match(nonOmocodeFC).Success) return false; // invalid Fiscal Code
        }
        return fc[15] == GetControlChar(fc.Substring(0, 15));

    }

    /// Checks for a Fiscal Code formal integrity using the following criteria:
    /// 
    /// - check for null/empty value
    /// - check for standard Regex [A-Z]{6}[\d]{2}[A-Z][\d]{2}[A-Z][\d]{3}[A-Z]
    /// - check for a valid Control Character
    /// - check it against given firstName, lastName, birthDate, gender and ISTATcode
    /// 
    /// NOTES:
    /// - Even if this function validates a Fiscal Code as OK, there are no guarrantees that the Fiscal Code is valid and existing.
    ///   The only way to check this is to use the official tools provided by "Agenzia delle Entrate", such as:
    ///   https://telematici.agenziaentrate.gov.it/VerificaCF/Scegli.do?parameter=verificaCf
    ///   Or other services that are connected to the "Agenzia delle Entrate" database and thus able to perform lookups there.
    /// <param name="fc">The Fiscal Code to check</param>
    /// <param name="firstName">The First Name to check the Fiscal Code against</param>
    /// <param name="lastName">The Last Name to check the Fiscal Code against</param>
    /// <param name="birthDate">The Birth Date to check the Fiscal Code against, together with gender.</param>
    /// <param name="gender">The Gender to check the Fiscal Code against, together with birthDate.</param>
    /// <param name="istatCode">The birthday city ISTAT code to check the Fiscal Code against</param>
    /// <returns>TRUE if the fiscal code is correct, FALSE otherwise</returns>
    /// </summary>
    public static bool IsValid(string fc, string firstName, string lastName, DateTime birthDate, char gender, string istatCode)
    {
        if (String.IsNullOrEmpty(fc) || fc.Length < 16) return false;
        fc = Normalize(fc, false);
        string nonOmocodeFC = string.Empty;
        if (!CheckRegex.Match(fc).Success)
        {
            // Regex failed: it can be either an omocode or an invalid Fiscal Code
            nonOmocodeFC = ReplaceOmocodeChars(fc);
            if (!CheckRegex.Match(nonOmocodeFC).Success) return false; // invalid Fiscal Code
        }
        else nonOmocodeFC = fc;

        // NOTES: 
        // - 'fc' is the given Fiscal Code (it might be omocodic)
        // - 'nonOmocodeFC' is the non-omocodic version of the given Fiscal Code (identical to 'fc' for non-omocodic Fiscal Codes)

        if (String.IsNullOrEmpty(firstName) || nonOmocodeFC.Substring(3, 3) != GetFirstNameCode(firstName)) return false;
        if (String.IsNullOrEmpty(lastName) || nonOmocodeFC.Substring(0, 3) != GetLastNameCode(lastName)) return false;
        if (nonOmocodeFC.Substring(6, 5) != GetBirthDateByGenderCode(birthDate, gender)) return false;
        if (String.IsNullOrEmpty(istatCode) || nonOmocodeFC.Substring(11, 4) != Normalize(istatCode, false)) return false;

        // We need to use 'fc' here instead of 'nonOmocodeFC': that's because the ControlChar is calculated using the omocode chars (if any).
        if (fc[15] != GetControlChar(fc.Substring(0, 15))) return false;

        // if we reach this, it means that the Fiscal Code is formally valid.
        return true;
    }

    /// <summary>
    /// Replace OmocodeChars (if present) to their corresponding numeric values
    /// </summary>
    /// <param name="fc">Fiscal Code potentially containing omocode chars</param>
    /// <returns></returns>
    public static string ReplaceOmocodeChars(string fc)
    {
        char[] fcChars = fc.ToCharArray();
        int[] pos = new[] { 6, 7, 9, 10, 12, 13, 14 };
        foreach (int i in pos) if (!Char.IsNumber(fcChars[i])) fcChars[i] = OmocodeChars.IndexOf(fcChars[i]).ToString()[0];
        return new string(fcChars);
    }
    #endregion Public Methods

    #region Private Methods
    /// <summary>
    /// Gets the first 3-letters of the name used to calculate the Fiscal Code.
    /// </summary>
    /// <param name="s">the person's Last Name (es. De Angelis)</param>
    /// <returns>the 3 FC-relevant letters for the name (es. DNG)</returns>
    private static string GetLastNameCode(string s)
    {
        s = Normalize(s, true);
        string code = string.Empty;
        int i = 0;

        // pick Consonants
        while ((code.Length < 3) && (i < s.Length))
        {
            for (int j = 0; j < Consonants.Length; j++)
            {
                if (s[i] == Consonants[j]) code += s[i];
            }
            i++;
        }
        i = 0;

        // pick Vocals (if needed)
        while (code.Length < 3 && i < s.Length)
        {
            for (int j = 0; j < Vocals.Length; j++)
            {
                if (s[i] == Vocals[j]) code += s[i];
            }
            i++;
        }

        // add trailing X (if needed)
        return (code.Length < 3) ? code.PadRight(3, 'X') : code;
    }

    /// <summary>
    /// Gets the first 3-letters of the name used to calculate the Fiscal Code.
    /// </summary>
    /// <param name="s">the person's name (es. Matteo)</param>
    /// <returns>the 3 FC-relevant letters for the name (es. MTT)</returns>
    private static string GetFirstNameCode(string s)
    {
        s = Normalize(s, true);
        string code = string.Empty;
        string cons = string.Empty;
        int i = 0;
        while ((cons.Length < 4) && (i < s.Length))
        {
            for (int j = 0; j < Consonants.Length; j++)
            {
                if (s[i] == Consonants[j]) cons = cons + s[i];
            }
            i++;
        }

        code = (cons.Length > 3)
            // if we have 4 or more consonants we need to pick 1st, 3rd and 4th
            ? cons[0].ToString() + cons[2].ToString() + cons[3].ToString()
            // otherwise we pick them all
            : code = cons;

        i = 0;
        // add Vocals (if needed)
        while ((code.Length < 3) && (i < s.Length))
        {
            for (int j = 0; j < Vocals.Length; j++)
            {
                if (s[i] == Vocals[j]) code += s[i];
            }
            i++;
        }

        // add trailing X (if needed)
        return (code.Length < 3) ? code.PadRight(3, 'X') : code;
    }

    /// <summary>
    /// Retrieves the Birth Date Code (by Gender)
    /// </summary>
    /// <param name="d">Birth Date</param>
    /// <param name="g">Gender (either 'M' or 'F')</param>
    /// <returns>the Birth Date Code (by Gender)</returns>
    private static string GetBirthDateByGenderCode(DateTime d, char g)
    {
        string code = d.Year.ToString().Substring(2);
        code += Months[d.Month - 1];
        if (g == 'M' || g == 'm') code += (d.Day <= 9) ? "0" + d.Day.ToString() : d.Day.ToString();
        else if (g == 'F' || g == 'f') code += (d.Day + 40).ToString();
        else throw new NotSupportedException("ERROR: Gender must be either 'M' or 'F'.");
        return code;
    }

    /// <summary>
    /// Retrieves the Control Character
    /// </summary>
    /// <param name="f15">the first 15 characters of the fiscal Code (full CF minus the Control Character only)</param>
    /// <returns>the Control Character</returns>
    private static char GetControlChar(string f15)
    {
        int tot = 0;
        byte[] arrCode = Encoding.ASCII.GetBytes(f15.ToUpper());
        for (int i = 0; i < f15.Length; i++)
        {
            if ((i + 1) % 2 == 0) tot += (char.IsLetter(f15, i))
                ? arrCode[i] - (byte)'A'
                : arrCode[i] - (byte)'0';
            else tot += (char.IsLetter(f15, i))
                ? ControlCodeArray[(arrCode[i] - (byte)'A')]
                : ControlCodeArray[(arrCode[i] - (byte)'0')];
        }
        tot %= 26;
        char l = (char)(tot + 'A');
        return l;
    }

    /// <summary>
    /// Trim, Uppercase and optionally remove diacritics to a string.
    /// </summary>
    private static string Normalize(string s, bool normalizeDiacritics)
    {
        if (String.IsNullOrEmpty(s)) return s;
        s = s.Trim().ToUpper();
        if (normalizeDiacritics)
        {
            string src = "ÀÈÉÌÒÙàèéìòù";
            string rep = "AEEIOUAEEIOU";
            for (int i = 0; i < src.Length; i++) s = s.Replace(src[i], rep[i]);
            return s;
        }
        return s;
    }
    #endregion Private Methods
}