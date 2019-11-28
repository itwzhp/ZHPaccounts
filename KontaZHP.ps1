#Fork of the user management script
#Author: Piotr Kolodziejski
#Email: admin@lodzka.zhp.pl, p.kolodziejski@gmail.com
#Based on script by: Przemyslaw Mikulski

#Original sript header:

#Below script has been created to internal use in ZHP organization
#Author: Przemyslaw Mikulski
#Hufiec ZHP Grojec
#Contact: mikul107@gmail.com ora przemyslaw.mikulski@zhp.net.pl
#
#Before first use open another Powershell window "as administrator" and install MSOnline module. Run command - "Install-Module -Name MSOnline"

#Logowanie
Write-Host 'Witaj w aplikacji do zakładania konto ZHP! Musisz się zalogować aby kontynuować pracę'
$mycreds = Get-Credential -Message 'Podaj dane do konta ZHP. Wymagane sa do tworzenia kont oraz wyslania meili' -UserName admin@zhp.pl
$mycredsLogin = $mycreds.UserName
try
{4
    Connect-MsolService -Credential $mycreds -ErrorAction Stop
    Write-Host ''
    Write-Host 'Podane dane się zgadzają. Uruchamianie programu...'
    Start-Sleep -s 2
}
catch 
{
    Write-Warning "Podanego hasło lub login są nieprawidłowe. Uruchom program ponownie. Jeśli używasz programu po raz pierwszy musisz zainstalować moduł 'MSOnline'. Aby to zrobić uruchom Powershell jako administrator i wykonaj polecenie 'Install-Module -Name MSOnline' "
    Start-Sleep -s 5
    Exit
}


####################################
# swap polish letters to latin
####################################

function LatinCharacters
{
    PARAM ([string]$String)
    [Text.Encoding]::ASCII.GetString([Text.Encoding]::GetEncoding("Cyrillic").GetBytes($String))
}

##################
# Send Mail
##################

Function SendMail($reciepent, $mailbody)
{
    try
    {
        Send-MailMessage -To $reciepent -Bcc "admin@lodzka.zhp.pl" -from $mycredsLogin  -Subject 'ZHP - konto Office365' -smtpserver smtp.office365.com -usessl -Credential $mycreds -Encoding UTF8 -Port 587 -BodyAsHtml $mailbody -ErrorAction Stop
        Write-host "Udalo się wysłać meila z hasłem na adres $reciepent" -ForegroundColor Green
        pause
    }
    catch
    {
        Write-Warning $_.Exception.Message
        Write-Warning "Wystąpił jednak problem z wysłaniem meila z hasłem na prywatny adres $reciepent. Upewnij się czy dobrze wpisałeś adres e-mail. Możesz skorzystać z opcji 'Reset hasła + wysłanie meila' "
        pause
    }
}

##################
# Find User
##################

Function FindUser($string = "")
{
    if($string -eq "")
    {
        $string = Read-Host -Prompt 'Kogo szukasz?'
    }
    Get-MsolUser -SearchString $string | Select-Object -Property FirstName, LastName, DisplayName, UserPrincipalName, Licenses, Office, Department, WhenCreated, LastPasswordChangeTimestamp|Format-List
}

##################
# Create User
##################

Function CreateUser
{
    $Name = Read-Host -Prompt 'Imie'
    $Surname = Read-Host -Prompt 'Nazwisko'

    $mailcandidate = $Name + "." + $Surname + "@zhp.net.pl"
    $mailcandidate = LatinCharacters $mailcandidate.ToLower()
    $usercheck = FindUser $mailcandidate
    If($usercheck -ne $Null)
    {
        Write-Host "Konto o tym imieniu i nazwisku istnieje. Sprawdź poniższą listę."
        FindUser $mailcandidate
        $_ = Read-Host "1. Czy chcesz zresetować hasło dla istniejącego konta? (t/n/0)"
        If($_ -eq "t"){
            UserPasswordReset
            continue
        }
        If($_ -eq 0){
            continue
        }
        else 
        {
            $mail = Read-Host -Prompt 'Nowy adres e-mail'
        }
    }
    Else 
    {
        $mail = $mailcandidate
    }
    Write-Host $mail
    $MalyMail = $mail.ToLower()
    $Hufiec = Read-Host -Prompt 'Hufiec'
    $DuzyHufiec = $Hufiec.ToUpper()
    $NrJednostki = Read-Host -Prompt 'Nr. jednostki'
    $privateMail = Read-Host -Prompt 'Podaj prywatny adres e-mail uzytkownika, na ktory ma zostac wyslane haslo'
    try
    {
        $a = New-MsolUser -UserPrincipalName $MalyMail -DisplayName "$Name $Surname" -FirstName $Name -LastName $Surname -LicenseAssignment "gkzhp:STANDARDWOFFPACK","gkzhp:FLOW_FREE","gkzhp:POWER_BI_STANDARD" -Department $NrJednostki -Office $DuzyHufiec -ForceChangePassword $true -PreferredLanguage "pl-PL" -PasswordNeverExpires $false -StrongPasswordRequired $true -UsageLocation PL -ErrorAction Stop
        Write-Host 'Super! Udało się stworzyć konto. Poniżej znajdziesz dane utworzonego konta. Możesz sprawdzić czy wszystko się zgadza.' -ForegroundColor Green
        Get-MsolUser -SearchString $MalyMail | Select-Object -Property FirstName, LastName, DisplayName, UserPrincipalName, Licenses, Office, Department, WhenCreated, LastPasswordChangeTimestamp|Format-List
    }
    catch
    {
        Write-Host "Hasło: $firstUserPwd"
        Write-Warning "Cos poszlo nie tak podczas tworzenia konta. Prawdopodobnie został źle podany adres e-mail ($mail) lub wystąpił problem z przypisaniem licencji. "
        Get-MsolUser -SearchString $MalyMail | Select-Object -Property FirstName, LastName, DisplayName, UserPrincipalName, Licenses, Office, Department, WhenCreated, LastPasswordChangeTimestamp|Format-List
        pause
        break
    }
    $firstUserPwd = ($a).password

    $bodyNew = "<p>Dzień dobry,</p>
<p>Właśnie założyłem dla Ciebie konto ZHP. Możesz się do niego zalogować przez stronę https://www.office.com.</p>
<p>- adres e-mail: <strong>$MalyMail</strong></p>
<p>- hasło do pierwszego logowania: <strong>$firstUserPwd</strong></p>
<p>Od teraz masz dostęp do aplikacji takich jak Word, Excel, Forms, Outlook i innych. Wszystkie dostępne są w wersji Online przez przeglądarkę.<p/>
<p>W celach bezpieczeństwa wymagana jest dwuetapowa weryfikacja. Oznacza to, że do zalogowania potrzebne będzie dodatkowe potwierdzenie tożsamości poprzez prywatny numer telefonu lub adres e-mail. <strong>Koniecznie uzupełnij wymagane dane podczas pierwszego logowania!</strong><p/>
<p>Jeśli w przyszłości zapomnisz hasła spróbuj w pierwszej kolejności samodzielnie odzyskać hasło, przez opcje 'Nie możesz uzyskać dostępu do konta?', która dostępna jest na stronie logowania.</p>
<p>Z harcerskim pozdrowieniem,<br />
Czuwaj!<br />
Piotr Kołodziejski<br />
Chorągiew Łódzka ZHP</p>"

    SendMail $privateMail $bodyNew
}

######################
# Reset license & pass
######################

Function UserPasswordLicenseReset
{
    $mail = Read-Host -Prompt 'Podaj adres e-mail uzytkownika w domenie ZHP'
    $privateMail = Read-Host -Prompt 'Podaj prywatny adres e-mail uzytkownika, na ktory ma zostac wyslane haslo'
    try
    {
        Set-MsolUserLicense -UserPrincipalName $mail -AddLicenses "gkzhp:STANDARDWOFFPACK", "gkzhp:FLOW_FREE", "gkzhp:POWER_BI_STANDARD" -ErrorAction SilentlyContinue
        $firstUserPwd = Set-MsolUserPassword -UserPrincipalName $mail -ForceChangePassword $true -ErrorAction Stop
        Write-Host "Super! Udało się przypisać licencje oraz zresetować hasło do konta $mail." -ForegroundColor Green
        Get-MsolUser -SearchString $mail | Select-Object -Property FirstName, LastName, DisplayName, UserPrincipalName, Licenses, Office, Department, WhenCreated, LastPasswordChangeTimestamp|Format-List
    }
    catch
    {
        Write-Host "Hasło: $firstUserPwd"
        Write-Warning "Coś poszlo nie tak. Prawdopodobnie został źle podany adres e-mail ($mail). Spróbuj jeszcze raz."
        pause
        break
    }

    $bodyNew = "<p>Dzień dobry,</p>
<p>Właśnie założyłem dla Ciebie konto ZHP. Możesz się do niego zalogować przez stronę https://www.office.com.</p>
<p>- adres e-mail: <strong>$MalyMail</strong></p>
<p>- hasło do pierwszego logowania: <strong>$firstUserPwd</strong></p>
<p>Od teraz masz dostęp do aplikacji takich jak Word, Excel, Forms, Outlook i innych. Wszystkie dostępne są w wersji Online przez przeglądarkę.<p/>
<p>W celach bezpieczeństwa wymagana jest dwuetapowa weryfikacja. Oznacza to, że do zalogowania potrzebne będzie dodatkowe potwierdzenie tożsamości poprzez prywatny numer telefonu lub adres e-mail. <strong>Koniecznie uzupełnij wymagane dane podczas pierwszego logowania!</strong><p/>
<p>Jeśli w przyszłości zapomnisz hasła spróbuj w pierwszej kolejności samodzielnie odzyskać hasło, przez opcje 'Nie możesz uzyskać dostępu do konta?', która dostępna jest na stronie logowania.</p>
<p>Z harcerskim pozdrowieniem,<br />
Czuwaj!<br />
Piotr Kołodziejski<br />
Chorągiew Łódzka ZHP</p>"

    SendMail $privateMail $bodyNew
}

##################
# Reset pass
##################


Function UserPasswordReset
{
    $mail = Read-Host -Prompt 'Podaj adres e-mail uzytkownika w domenie ZHP'
    $privateMail = Read-Host -Prompt 'Podaj prywatny adres e-mail uzytkownika, na ktory ma zostac wyslane haslo'
    try
    {
    $firstUserPwd = Set-MsolUserPassword -UserPrincipalName $mail -ForceChangePassword $true -ErrorAction Stop
        Write-Host "Super! Udało się zresetować hasło do konta $mail." -ForegroundColor Green
    }
    catch
    {
        Write-Host "Hasło: $firstUserPwd"
        Write-Warning "Coś poszlo nie tak. Prawdopodobnie został źle podany adres e-mail ($mail). Spróbuj jeszcze raz."
        pause
        break
    }

    $bodyReset = "<p>Dzień dobry,</p>
<p>Twoje hasło do konta ZHP właśnie zostało zresetowane. Możesz zalogować się ponownie przez stronę https://www.office.com, używając poniższych danych:</p>
<p>- adres e-mail: <strong>$mail</strong></p>
<p>- hasło do pierwszego logowania: <strong>$firstUserPwd</strong></p>
<p>W celach bezpieczeństwa wymagana jest dwuetapowa weryfikacja. Oznacza to, że do zalogowania potrzebne będzie dodatkowe potwierdzenie tożsamości poprzez prywatny numer telefonu lub adres e-mail. <strong>Koniecznie uzupełnij wymagane dane podczas pierwszego logowania!</strong><p/>
<p>Jeśli w przyszłości zapomnisz hasła spróbuj w pierwszej kolejności samodzielnie odzyskać hasło, przez opcje 'Nie możesz uzyskać dostępu do konta?', która dostępna jest na stronie logowania.</p>
<p>Z harcerskim pozdrowieniem,<br />
Czuwaj!<br />
Piotr Kołodziejski<br />
Chorągiew Łódzka ZHP</p>"

    SendMail $privateMail $bodyReset
}

##################
# Main Menu
##################

Function Menu 
{
    Clear-Host        
    Do
    {
        Clear-Host                                                                       
        Write-Host -Object 'Tworzenie konta ZHP' -ForegroundColor Yellow
        Write-Host -Object '**********************'
        Write-Host -Object '1.  Szukaj uzytkownika'
        Write-Host -Object ''
        Write-Host -Object '2.  Tworzenie konta'
        Write-Host -Object ''
        Write-Host -Object '3.  Update licencji + reset hasła + wysłanie meila '
        Write-Host -Object ''
        Write-Host -Object '4.  Reset hasła + wysłanie meila '
        Write-Host -Object ''
        Write-Host -Object 'Q.  Wyjście'
        Write-Host -Object $errout
        $Menu = Read-Host -Prompt '(0-4 lub Q aby wyjść)'
 
        switch ($Menu) 
        {
            1 
            {
                FindUser
                Pause
            }

            2 {CreateUser}
            3 {UserPasswordLicenseReset}
            4 {UserPasswordReset}
            Q {Exit}   
            default
            {
                $errout = 'Nie ma takiego programu. Spróbuj jeszcze raz........ 0-4 lub Q aby wyjść'
            }
        }
    }
    until ($Menu -eq 'q')
}   
 
# Launch The Menu
Menu