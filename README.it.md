Classe helper C# per il controllo e il calcolo formale del Codice Fiscale. Il codice è stato creato da [Ryan](https://www.ryadel.com/author/ryan/) e rilasciato su [ryadel.com](https://www.ryadel.com/en/italian-tax-code-fiscal-code-vat-id-c-sharp-class/).

Ci sono molte risorse online che mostrano come validare un codice fiscale, ma tutte queste risorse usano lo stesso approccio. Il codice controlla che il codice fiscale sia valido con una chiamata API ad un server che alcune volte funziona oppure utilizza un grosso file csv con tutte le citta' d'Italia per validare la stringa. Tutte queste funzionalita' possono solo funzionare in alcune situazioni e il codice non è ben ottimizzato.

Questa classe puo' funzionare perfettamente con ogni applicazione e fa tutto in una singola classe senza utilizzare risorse esterne.

Tutti i meriti vanno all'autore originale, questo repository serve solo per condividere il codice che non ho trovato velocemente facendo una ricerca su Google, specialmente su Github.

La solution ha due classi scritte in inglese e in italiano in modo da capire i metodi ed i parametri utilizzati.

Il codice ISTAT può essere trovato su [dait.interno.gov.it](https://dait.interno.gov.it/territorio-e-autonomie-locali/sut/elenco_codici_comuni.php), nell'ultima colonna a destra denominata come 'CODICE BELFIORE'.
