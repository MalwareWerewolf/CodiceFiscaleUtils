For all italian readers, there is another [README](https://github.com/MalwareWerewolf/CodiceFiscaleUtils/blob/main/README.it.md) written in italian in this repo.

A useful C# helper class to check for the formal validity of any Italian Tax Code / Fiscal Code / VAT ID. The code was made by [Ryan](https://www.ryadel.com/en/author/ryan/) and released on [ryadel.com](https://www.ryadel.com/en/italian-tax-code-fiscal-code-vat-id-c-sharp-class/).

There are many resources online which show how to validate the Fiscal Code, but all these resources use the same approach. The code checks if the fiscal code is valid with an API request to a server which sometimes doesn't work or it uses a big csv file with all the towns in Italy to validate the string. All these features can only work in some situations and the code is not well optimized.

This helper class can perfectly work in every application and it does everything in a single class without using external resources.

All the credits go the original author, this is just a repository to share his code that I didn't find so quickly while googling, especially on Github.

The solution has two classes written in english and in italian in order to understand the methods and the parameters used.

The ISTAT Code can be found on [dait.interno.gov.it](https://dait.interno.gov.it/territorio-e-autonomie-locali/sut/elenco_codici_comuni.php), in the last column on the right named as 'CODICE BELFIORE'.
