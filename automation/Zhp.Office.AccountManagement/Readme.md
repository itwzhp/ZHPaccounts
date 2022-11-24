# Automatyzacja zakładania kont ZHP
Aplikacja Azure Functions automatyzująca wykonanie zadań związanych z Microsoft365 jak zakładanie kont i resety haseł.
## Opis
Obecnie apliacja zawiera następujące funkcje:
- `CreateAccounts` - pobiera listę ticketów w stanie _Accepted_, zakłada dla nich konta i je zamyka
## Konfiguracja środowiska
Jako, że `dotnet user-secrets` zapisuje poświadczenia w sposób nieszyfrowany, należy go używać tylko na szyfrowanych dyskach.
Aby uruchomić aplikację lokalnie należy zainstalować .NET w wersji 6.0, a następnie w folderze projektu `Zhp.Office.AccountManagement` dodać przy pomocy poleceń `dotnet user-secrets` następujące klucze:
- `Jira:User` - nazwa użytkownika Jira
- `Jira:Password` - hasło użytkownika Jira
Po uruchomieniu w środowisku deweoloperskim aplikacja otworzy przeglądarkę, aby poprosić o uprawnienia do dostępu do Active Directory.
W środowisku produkcyjnym, aplikacja używa certyfikatu, aby uzyskać ten dostęp automatycznie. Do tego potrzebuje następujących kluczy:
- `ActiveDirectory:ProdCertificateBase64` - zakodowany w Base64 certyfikat PFX
- `ActiveDirectory:ProdCertPassword` - Hasło szyfrujące certyfikat
