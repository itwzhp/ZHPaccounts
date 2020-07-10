<#

.SYNOPSIS
To jest skrypt pomagajacy zarzadzajacy kontami w O365

.DESCRIPTION
To jest skrypt edytujacy zarzadzajacy kontami w O365 na bazie wskazanego pliku csv, 
Kolumny na bazie wykesportowanych kolumn z helpdesk.zhp.pl
Pole własne (Name),Pole własne (Surname),Pole własne (E-mail address),Pole własne (Hufiec),Pole własne (Numer członkowski),Pole własne (Numer członkowski (zgłaszającego))
Pola opcjonalne: Klucz zgłoszenia,Id. zgłoszenia,Typ zgłoszenia,Status,Customer Request Type,Utworzono,Zaktualizowano,

#>
<#
Copyright 2019 Andrzej Żmijewski

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#>

Param(
    [Parameter(HelpMessage = 'Sciezka do arkusza z danymi uzytkownikow')] [string]$Path,
    [Parameter(HelpMessage = 'Sciazka do pliku z haslem dla aplikacji do poczty O365 - jak wygenreowac szczegoly w kodzie')][string]$MailPassFile = "C:\Users\andrz\MailPassword.txt",
    [Parameter(HelpMessage = 'Parsuje wszsytkie rekordy na raz')][switch]$All = $false,
    [Parameter(HelpMessage = 'Definiuje kto ma byc w polu DoWiadomosci')][string]$KopiaDo = "aktywacja@gdanska.zhp.pl",
    [Parameter(HelpMessage = 'Definiuje kto ma byc nadawca')][string]$Od = "andrzej.zmijewski@zhp.net.pl"
)
 
[System.IO.Path]::HasExtension($Path)


#Zmienne globalne
$commands="Znajdź konta dla imienia i nazwiska","Znajdź konto dla emila","Zaproponuj inny adres","Reset hasła","Stwórz konto","Dodaj Licencje","Wyjdź"
$global:cc= $KopiaDo

<#wygenerowany uzywajac komend (trzymaj go w prywatnej przestrzeni i nie udostepniaj):
$Secure = Read-Host -AsSecureString #Podaj unikalne haslo wygenerowane dla aplikacji w opjach zabezpieczen O365
$Encrypted = ConvertFrom-SecureString -SecureString $Secure
Set-Content -Path $global:mailpassfile  -Value $Encrypted
#>
$global:mailpassfile = $MailPassFile
$global:username = $Od

$global:file = $Path
$global:debug = $False



function GetMailCretentials {
    $SecureStringPassword = "";
    if (-not $global:mailcredentials)
    {
        if ([System.IO.File]::Exists($global:mailpassfile )){
            #genreted using command:
            #$Secure = Read-Host -AsSecureString
            #$Encrypted = ConvertFrom-SecureString -SecureString $Secure
            #Set-Content -Path $global:mailpassfile  -Value $Encrypted
            $SecureStringPassword = Get-Content -Path $global:mailpassfile  | ConvertTo-SecureString

     
        }else{
            Write-Host "Podaj haslo do konta $($global.username):"
            $SecureStringPassword = Read-Host -AsSecureString
        }
        $global:mailcredentials = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $global:username,$SecureStringPassword
    }
    return  $global:mailcredentials
}

function SendMailWithPassword ($konto, $hasloDoWyslania, $mailcredentials){
    $mail = $konto.'Email ZHP'
	$nazwaWyswietlana="$($konto.'Pole własne (Name)') $($konto.'Pole własne (Surname)')"
    $From = $global:username
    $To = $konto.'Pole własne (E-mail address)'
    Write-Host "Wysylam maila do $($To)"
    $Subject = "Dostęp do eZHP"
    
	
$Body = @"
<html>

<head>
<meta http-equiv=Content-Type content="text/html; charset=windows-1252">
<meta name=Generator content="Microsoft Word 15 (filtered)">
<style>
<!--
 /* Font Definitions */
 @font-face
	{font-family:Wingdings;
	panose-1:5 0 0 0 0 0 0 0 0 0;}
@font-face
	{font-family:"Cambria Math";
	panose-1:2 4 5 3 5 4 6 3 2 4;}
@font-face
	{font-family:Calibri;
	panose-1:2 15 5 2 2 2 4 3 2 4;}
@font-face
	{font-family:"Segoe UI";
	panose-1:2 11 5 2 4 2 4 2 2 3;}
@font-face
	{font-family:"Trebuchet MS";
	panose-1:2 11 6 3 2 2 2 2 2 4;}
 /* Style Definitions */
 p.MsoNormal, li.MsoNormal, div.MsoNormal
	{margin-top:0cm;
	margin-right:0cm;
	margin-bottom:8.0pt;
	margin-left:0cm;
	line-height:107%;
	font-size:11.0pt;
	font-family:"Calibri",sans-serif;}
a:link, span.MsoHyperlink
	{color:#0563C1;
	text-decoration:underline;}
a:visited, span.MsoHyperlinkFollowed
	{color:#954F72;
	text-decoration:underline;}
p.MsoAcetate, li.MsoAcetate, div.MsoAcetate
	{mso-style-link:"Tekst dymka Znak";
	margin:0cm;
	margin-bottom:.0001pt;
	font-size:9.0pt;
	font-family:"Segoe UI",sans-serif;}
p.MsoListParagraph, li.MsoListParagraph, div.MsoListParagraph
	{margin-top:0cm;
	margin-right:0cm;
	margin-bottom:8.0pt;
	margin-left:36.0pt;
	line-height:107%;
	font-size:11.0pt;
	font-family:"Calibri",sans-serif;}
p.MsoListParagraphCxSpFirst, li.MsoListParagraphCxSpFirst, div.MsoListParagraphCxSpFirst
	{margin-top:0cm;
	margin-right:0cm;
	margin-bottom:0cm;
	margin-left:36.0pt;
	margin-bottom:.0001pt;
	line-height:107%;
	font-size:11.0pt;
	font-family:"Calibri",sans-serif;}
p.MsoListParagraphCxSpMiddle, li.MsoListParagraphCxSpMiddle, div.MsoListParagraphCxSpMiddle
	{margin-top:0cm;
	margin-right:0cm;
	margin-bottom:0cm;
	margin-left:36.0pt;
	margin-bottom:.0001pt;
	line-height:107%;
	font-size:11.0pt;
	font-family:"Calibri",sans-serif;}
p.MsoListParagraphCxSpLast, li.MsoListParagraphCxSpLast, div.MsoListParagraphCxSpLast
	{margin-top:0cm;
	margin-right:0cm;
	margin-bottom:8.0pt;
	margin-left:36.0pt;
	line-height:107%;
	font-size:11.0pt;
	font-family:"Calibri",sans-serif;}
span.TekstdymkaZnak
	{mso-style-name:"Tekst dymka Znak";
	mso-style-link:"Tekst dymka";
	font-family:"Segoe UI",sans-serif;}
span.UnresolvedMention
	{mso-style-name:"Unresolved Mention";
	color:#605E5C;
	background:#E1DFDD;}
.MsoChpDefault
	{font-family:"Calibri",sans-serif;}
.MsoPapDefault
	{margin-bottom:8.0pt;
	line-height:107%;}
@page WordSection1
	{size:612.0pt 792.0pt;
	margin:72.0pt 72.0pt 72.0pt 72.0pt;}
div.WordSection1
	{page:WordSection1;}
 /* List Definitions */
 ol
	{margin-bottom:0cm;}
ul
	{margin-bottom:0cm;}
-->
</style>

</head>

<body lang=PL link="#0563C1" vlink="#954F72">

<div class=WordSection1>

<p class=MsoNormal style='margin-bottom:6.0pt'><span style='font-size:10.5pt;
line-height:107%;font-family:"Trebuchet MS",sans-serif'>Druhno/Druhu,</span></p>

<p class=MsoNormal style='margin-bottom:6.0pt;line-height:normal'><span
style='font-size:10.5pt;font-family:"Trebuchet MS",sans-serif'>w&#322;a&#347;nie
za&#322;o&#380;yli&#347;my dla Ciebie konto ZHP. Mo&#380;esz si&#281; do niego
zalogowa&#263; przez stron&#281; <span class=MsoHyperlink><a
href="https://www.office.com">https://www.office.com</a></span> lub <span
class=MsoHyperlink><a href="https://poczta.zhp.pl.&nbsp;">https://poczta.zhp.pl.&nbsp;</a></span></span></p>

<p class=MsoNormal><span style='font-size:10.5pt;line-height:107%;font-family:
"Trebuchet MS",sans-serif'>&nbsp;</span></p>

<p class=MsoNormal><span style='font-size:10.5pt;line-height:107%;font-family:
"Trebuchet MS",sans-serif'>Szczegó&#322;y u&#380;ytkownika</span><span
lang=EN-US><br>
</span><span style='font-size:10.5pt;line-height:107%;font-family:"Trebuchet MS",sans-serif'> Nazwa
wy&#347;wietlana: </span><b><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif;color:#0B5394">$nazwaWyswietlana</span></b><br>
<span style='font-size:10.5pt;line-height:107%;font-family:"Trebuchet MS",sans-serif'> Nazwa
u&#380;ytkownika: </span><b><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif;color:#0B5394">$mail</span></b><br>
<span style='font-size:10.5pt;line-height:107%;font-family:"Trebuchet MS",sans-serif'> Has&#322;o:</span><b><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif;color:#0B5394"> $hasloDoWyslania</span></b></p>

<p class=MsoNormal style='margin-bottom:6.0pt;line-height:normal'><span
style='font-size:10.5pt;font-family:"Trebuchet MS",sans-serif'>&nbsp;</span></p>

<p class=MsoNormal style='margin-bottom:6.0pt;line-height:normal'><span
style='font-size:10.5pt;font-family:"Trebuchet MS",sans-serif'>Od teraz masz
dost&#281;p do aplikacji takich jak Outlook, Teams, Word, Excel, Forms i
innych. S&#261; one dost&#281;pne w wersji Online przez przegl&#261;dark&#281;.</span></p>

<p class=MsoNormal style='margin-bottom:6.0pt;line-height:normal'><span
style='font-size:10.5pt;font-family:"Trebuchet MS",sans-serif'>W celach
bezpiecze&#324;stwa mo&#380;e by&#263; wymagana dwuetapowa weryfikacja. Oznacza
to, &#380;e do zalogowania potrzebne b&#281;dzie dodatkowe potwierdzenie
to&#380;samo&#347;ci poprzez prywatny numer telefonu, adres e-mail lub
aplikacj&#281; na telefon. Koniecznie uzupe&#322;nij wymagane dane podczas
pierwszego logowania!</span></p>

<p class=MsoNormal style='margin-bottom:6.0pt;line-height:normal'><span
style='font-size:10.5pt;font-family:"Trebuchet MS",sans-serif'>Je&#347;li w
przysz&#322;o&#347;ci zapomnisz has&#322;a spróbuj w pierwszej kolejno&#347;ci
samodzielnie odzyska&#263; has&#322;o, przez opcj&#281; 'Nie mo&#380;esz
uzyska&#263; dost&#281;pu do konta?', która dost&#281;pna jest na stronie
logowania.</span></p>

<p class=MsoNormal style='margin-bottom:6.0pt;line-height:normal'><span
style='font-size:10.5pt;font-family:"Trebuchet MS",sans-serif'>Zostawi&#281;
te&#380; kilka przydatnych linków, dzi&#281;ki którym &#322;atwiej b&#281;dzie
Ci si&#281; odnale&#378;&#263; w naszej organizacji.</span></p>

<p class=MsoListParagraphCxSpFirst style='margin-bottom:6.0pt;text-indent:-18.0pt;
line-height:normal'><span style='font-size:10.5pt;font-family:Symbol'>·<span
style='font:7.0pt "Times New Roman"'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span class=MsoHyperlink><span style='font-size:10.5pt;
font-family:"Trebuchet MS",sans-serif'><a href="https://intranet.zhp.pl">https://intranet.zhp.pl</a></span></span><span
style='font-size:10.5pt;font-family:"Trebuchet MS",sans-serif'> – harcerski intranet,
czyli wewn&#281;trzna witryna z aktualno&#347;ciami, dokumentami i innymi
materia&#322;ami dla harcerskiej kadry,</span></p>

<p class=MsoListParagraphCxSpMiddle style='margin-bottom:6.0pt;text-indent:
-18.0pt;line-height:normal'><span style='font-size:10.5pt;font-family:Symbol'>·<span
style='font:7.0pt "Times New Roman"'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span class=MsoHyperlink><span style='font-size:10.5pt;
font-family:"Trebuchet MS",sans-serif'><a href="https://tipi.zhp.pl">https://tipi.zhp.pl</a></span></span><span
style='font-size:10.5pt;font-family:"Trebuchet MS",sans-serif'> – system ewidencji
naszej organizacji - je&#347;li chcia&#322;by&#347; mie&#263; tam dost&#281;p
zg&#322;o&#347; si&#281; do swojego dru&#380;ynowego lub administratora
chor&#261;gwi,</span></p>

<p class=MsoListParagraphCxSpMiddle style='margin-bottom:6.0pt;text-indent:
-18.0pt;line-height:normal'><span style='font-size:10.5pt;font-family:Symbol'>·<span
style='font:7.0pt "Times New Roman"'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span class=MsoHyperlink><span style='font-size:10.5pt;
font-family:"Trebuchet MS",sans-serif'><a href="https://pomoc.zhp.pl">https://pomoc.zhp.pl</a></span></span><span
style='font-size:10.5pt;font-family:"Trebuchet MS",sans-serif'> – strona, na
której mo&#380;esz znale&#378;&#263; przydatne artyku&#322;y pomocy w razie
problemów podczas pracy z wewn&#281;trznymi systemami jak Microsoft 365, TiPi
itp.</span></p>

<p class=MsoListParagraphCxSpLast style='margin-bottom:6.0pt;text-indent:-18.0pt;
line-height:normal'><span style='font-size:10.5pt;font-family:Symbol'>·<span
style='font:7.0pt "Times New Roman"'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span><span class=MsoHyperlink><span style='font-size:10.5pt;
font-family:"Trebuchet MS",sans-serif'><a href="https://helpdesk.zhp.pl">https://helpdesk.zhp.pl</a></span></span><span
style='font-size:10.5pt;font-family:"Trebuchet MS",sans-serif'> – serwis do
zg&#322;aszania problemów i pyta&#324; dotycz&#261;cych harcerskiego IT.&nbsp;</span></p>

<p class=MsoNormal style='margin-top:0cm;margin-right:0cm;margin-bottom:6.0pt;
margin-left:18.0pt;line-height:normal'><span style='font-size:10.5pt;
font-family:"Trebuchet MS",sans-serif'>&nbsp;</span></p>

<p class=MsoNormal><span style='font-size:10.5pt;line-height:107%;font-family:
"Trebuchet MS",sans-serif'>Szkolenia z narz&#281;dzi pakietu Office365
mo&#380;na znale&#378;&#263; tutaj:</span><span lang=EN-US><br>
</span><span lang=EN-US style='font-size:10.5pt;line-height:107%;font-family:
"Trebuchet MS",sans-serif'> </span><span class=MsoHyperlink><span
style='font-size:10.5pt;line-height:107%;font-family:"Trebuchet MS",sans-serif'>https://support.office.com/pl-pl/office-training-center</span></span></p>

<p class=MsoNormal style='margin-bottom:6.0pt;line-height:normal'><span
style='font-size:10.5pt;font-family:"Trebuchet MS",sans-serif'>Udanego
korzystania </span><span lang=EN-US style='font-size:10.5pt;font-family:"Trebuchet MS",sans-serif'>&#128578;</span><span
lang=EN-US><br>
</span><span style='font-size:10.5pt;font-family:"Trebuchet MS",sans-serif'>Czuwaj!</span><span
lang=EN-US><br>
<br>
</span></p>

</div>

</body>

</html>
"@

$SMTPServer = "smtp.office365.com"
$SMTPPort = "587"
Send-MailMessage -Encoding UTF8 -From $From -to $To -Cc $global:cc -Subject $Subject -Body $Body -BodyAsHtml -SmtpServer $SMTPServer -Port $SMTPPort -UseSsl -Credential $mailcredentials
}


####################################
# swap polish letters to latin
####################################
function LatinCharacters
{
    PARAM ([string]$String)
    [Text.Encoding]::ASCII.GetString([Text.Encoding]::GetEncoding("Cyrillic").GetBytes($String))
}

function getKeyResponse ($request){
    $res = ""
    do {
        $res = Read-Host -Prompt $request    
    } while (-not ($res.Length -eq 1) )
    return    $res[0]
    #return $HOST.UI.RawUI.ReadKey().VirtualKeyCode - 49
}


function AskConfirmation([string]$Question)
{
    return ((getKeyResponse "${Question}? (y/n)") -eq "y")
    #return $HOST.UI.RawUI.ReadKey().VirtualKeyCode -eq 89
}

function LoadData()
{
    if (-not $global:file)
    {
        $FileBrowser = New-Object System.Windows.Forms.OpenFileDialog 
        $null = $FileBrowser.ShowDialog()
        $global:file = $FileBrowser.FileName 
    }
    $stores = Import-Csv -Path $global:file

    return $stores
}

function CheckData($konta)
{
    $wymagane = @('Pole własne (Name)', 'Pole własne (Surname)', 'Pole własne (Chorągiew)', 'Pole własne (Hufiec)', 'Pole własne (E-mail address)') 
    #"Email ZHP" is created by default
    $konta | ForEach-Object { 
        TrimValues $_
        $konto = $_
        Write-Host Sprawdzam rekod $konto
        $wymagane | ForEach-Object  {
            if ([bool]($konto.PSobject.Properties.name -notcontains $_)){
                throw [System.MissingFieldException] "Pole $_ nie istnieje dla rekordu $($konto)."
            }    
        }
        if ([bool]($konto.PSobject.Properties.name -notcontains "Email ZHP")){ 
            $konto | Add-Member -MemberType NoteProperty -Name "Email ZHP" -Value "$($_.'Pole własne (Name)'.ToLower()).$($_.'Pole własne (Surname)'.ToLower())@zhp.net.pl"
        }
    }
}

function AskForCommands($commands)
{
    #Write-Host "Wybierz komendę: "
    $commands | ForEach-Object {$i = 1} { Write-Host "$i -  $_"; $i++ }
    return [int](getKeyResponse "Wybierz komendę ") - 49
    #return $HOST.UI.RawUI.ReadKey().VirtualKeyCode - 49
}

function ZnajdzKontaDlaNazwiska($konto)
{
    Write-Output "Znalazlem:"
    Get-MsolUser -SearchString "$($_.'Pole własne (Name)') $($konto.'Pole własne (Surname)')" | select FirstName, LastName, Department, Office, SignInName | Sort FirstName | Format-Table
    Write-Output "####################"
}

function ZnajdźKontoDlaEmila($email)
{
    Write-Output "Email $($email) ma przypisane konto:"
    Get-MsolUser -UserPrincipalName "$($email)" | select FirstName, LastName, Department, Office, SignInName |  Format-Table
    Write-Output "####################"
}

function ZaproponujInnyAdres($konto)
{
    $emails = @(
    "$($_.'Pole własne (Name)'.ToLower()).$($_.'Pole własne (Surname)'.ToLower())@zhp.net.pl",
    "$($_.'Pole własne (Surname)'.ToLower()).$($_.'Pole własne (Name)'.ToLower())@zhp.net.pl",
    "$($_.'Pole własne (Name)'.ToLower()[0]).$($_.'Pole własne (Surname)'.ToLower())@zhp.net.pl",
    "$($_.'Pole własne (Surname)'.ToLower()).$($_.'Pole własne (Name)'.ToLower()[0])@zhp.net.pl")
	$emails | ForEach-Object -Process {$a = LatinCharacters $_; ZnajdźKontoDlaEmila $a}
    $email = AskForCommands $emails
    $konto.'Email ZHP' = $emails[$email]
}

function ResetHasla ($konto) {
    $length = 10 ## characters
    $nonAlphaChars = 3
    $password = [System.Web.Security.Membership]::GeneratePassword($length, $nonAlphaChars)

    $mailcredentials = GetMailCretentials
    Write-Host "Resetuje haslo dla $($konto."Email ZHP"). Nowe haslo: $password"
    Set-MsolUserPassword -UserPrincipalName $konto."Email ZHP" -NewPassword $password -ForceChangePassword $True
    SendMailWithPassword $konto $password $mailcredentials
}

function AddLicense ($konto) {
    Set-MsolUserLicense -UserPrincipalName -UserPrincipalName $konto."Email ZHP" AddLicenses 'gkzhp:STANDARDWOFFPACK','gkzhp:POWER_BI_STANDARD'
}


function StworzKonto ($konto) {
    $mailcredentials = GetMailCretentials
    Write-Host "Zakladam konto $konto"
    $uzytokownik = New-MsolUser -UserPrincipalName $konto."Email ZHP" -FirstName $konto.'Pole własne (Name)' -LastName $konto.'Pole własne (Surname)' -DisplayName "$($konto.'Pole własne (Name)') $($konto.'Pole własne (Surname)')" -Office "Chorągiew $($konto.'Pole własne (Chorągiew)')" -Department "Hufiec $($konto.'Pole własne (Hufiec)')" -LicenseAssignment 'gkzhp:STANDARDWOFFPACK','gkzhp:POWER_BI_STANDARD' -UsageLocation PL
    SendMailWithPassword $konto $uzytokownik.Password $mailcredentials
    return $uzytokownik
}

function TrimValues($konto)
{
    $konto.PSObject.Properties | ForEach-Object { $konto.($_.Name) = $konto.($_.Name).trim()}
}

function sendTestMail {
    $mailcredentials = GetMailCretentials
    $konto=@{'Email ZHP' = "andrzej.zmijewski@zhp.net.pl";
            'Pole własne (Name)' = "Andrzej";
            'Pole własne (Surname)'="zMIJEWSKI";
            'Pole własne (E-mail address)'="andrzej.zmijewski@zhp.net.pl"}

    SendMailWithPassword $konto haslo123 $mailcredentials
    throw "Exit"
}

##################################################

$currentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
$global:isAdmin = $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Drawing") 
[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms") 

#$global:credentials = Get-Credential $global:username

Import-Module MsOnline
Connect-MsolService #-Credential $credentials

$konta = LoadData  
CheckData $konta


if ($konta.Length -eq 0)
{
    throw "Brak danych do przetworzenia"
} 

$toExit = $False

while(-not $toExit)
{

    $command = AskForCommands $commands 
	
    if ($commands[$command] -eq "Wyjdź") { $toExit = $True ; continue}
    $runForAll = $False

    if ( $All-or $global:debug )
    {
        $runForAll = $True
    }
    else
    {
        $runForAll = AskConfirmation "Wykonac komende dla wszykich kont"
    }

    $konta | ForEach-Object  {

        $_.'Email ZHP' = LatinCharacters $_.'Email ZHP'.ToLower()
        
        Write-Host "Proceduje: $($_.'Email ZHP')"
        $shouldRun = $True
        if (-not $runForAll) { $shouldRun = ( AskConfirmation "Czy wykonac komende dla $($_.'Email ZHP')" ) }
        if ($shouldRun)
        {
            if ($commands[$command] -eq "Znajdź konta dla imienia i nazwiska") {ZnajdzKontaDlaNazwiska $_}
            elseif($commands[$command] -eq "Znajdź konto dla emila") {ZnajdźKontoDlaEmila $_.'Email ZHP'}
            elseif($commands[$command] -eq "Zaproponuj inny adres") {ZaproponujInnyAdres $_}
            elseif($commands[$command] -eq "Reset hasła") { ResetHasla $_ }
            elseif($commands[$command] -eq "Stwórz konto") { StworzKonto $_ }
            elseif($commands[$command] -eq "Dodaj Licencje") { AddLicense $_ }
            else { throw "nieznan komenda" }
        }
    }
}

# 
#for ($row = 2; $row -le 6; $row++)
#{
#    Write-Output $sheet1.cells.Item($row,8).Text
#    Write-Output $sheet1.cells.Item($row,10).Text
#    Invoke-Expression $sheet1.cells.Item($row,8).Text
#}

