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
$global:PodpisAdministratora="phm.&nbsp;Andrzej  &#379;mijewski"  #Polskie znaki: http://hektor.umcs.lublin.pl/~awmarczx/awm/info/pl-codes.htm
$global:FunkcjaAdministratora="Cz&#322;onek zespo&#322;u ds. informacji&nbsp;Chor&#261;gwi Gda&#324;skiej ZHP"

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
    $From = $global:username
    $To = $konto.'Pole własne (E-mail address)'
    Write-Host "Wysylam maila do $($To)"
    $Subject = "Dostęp do eZHP"
    $Body = '<META HTTP-EQUIV="Content-Type" CONTENT="text/html; charset=utf-8">
<html xmlns:v="urn:schemas-microsoft-com:vml" xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:w="urn:schemas-microsoft-com:office:word" xmlns:m="http://schemas.microsoft.com/office/2004/12/omml" xmlns="http://www.w3.org/TR/REC-html40"><head><meta name=Generator content="Microsoft Word 15 (filtered medium)"><style><!--
/* Font Definitions */
@font-face
	{font-family:"Cambria Math";
	panose-1:2 4 5 3 5 4 6 3 2 4;}
@font-face
	{font-family:Calibri;
	panose-1:2 15 5 2 2 2 4 3 2 4;}
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
	font-family:"Calibri",sans-serif;
	mso-fareast-language:EN-US;}
a:link, span.MsoHyperlink
	{mso-style-priority:99;
	color:#0563C1;
	text-decoration:underline;}
a:visited, span.MsoHyperlinkFollowed
	{mso-style-priority:99;
	color:#954F72;
	text-decoration:underline;}
p
	{mso-style-priority:99;
	mso-margin-top-alt:auto;
	margin-right:0cm;
	mso-margin-bottom-alt:auto;
	margin-left:0cm;
	font-size:12.0pt;
	font-family:"Times New Roman",serif;
	mso-fareast-language:PL;}
.MsoChpDefault
	{mso-style-type:export-only;
	font-family:"Calibri",sans-serif;}
.MsoPapDefault
	{mso-style-type:export-only;
	margin-bottom:8.0pt;
	line-height:107%;}
@page WordSection1
	{size:595.3pt 841.9pt;
	margin:70.85pt 70.85pt 70.85pt 70.85pt;}
div.WordSection1
	{page:WordSection1;}
--></style><!--[if gte mso 9]><xml>
<o:shapedefaults v:ext="edit" spidmax="1026" />
</xml><![endif]--><!--[if gte mso 9]><xml>
<o:shapelayout v:ext="edit">
<o:idmap v:ext="edit" data="1" />
</o:shapelayout></xml><![endif]--></head>
<body lang=EN-US link="#0563C1" vlink="#954F72"><div class=WordSection1>
<p class=MsoNormal style="margin-bottom:0cm;margin-bottom:.0001pt;line-height:normal"><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif">Czuwaj!<o:p></o:p></span></p>
<p class=MsoNormal style="margin-bottom:0cm;margin-bottom:.0001pt;line-height:normal"><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif"><o:p>&nbsp;</o:p></span></p>
<p class=MsoNormal style="margin-bottom:0cm;margin-bottom:.0001pt;line-height:normal"><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif">Twoje dane dost&#281;powe do eZHP to:<o:p></o:p></span></p>
<p class=MsoNormal style="margin-bottom:0cm;margin-bottom:.0001pt;line-height:normal"><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif">e-mail (i zarazem login): </span>
<b><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif;color:#0B5394">'+$mail+'</span></b><b><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif"><o:p></o:p></span></b></p>
<p class=MsoNormal style="margin-bottom:0cm;margin-bottom:.0001pt;line-height:normal"><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif">has&#322;o:</span>
<b><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif;color:#0B5394"> '+$hasloDoWyslania+'</span></b><b><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif"><o:p></o:p></span></b></p><p class=MsoNormal style="margin-bottom:0cm;margin-bottom:.0001pt;line-height:normal"><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif"><o:p>&nbsp;</o:p></span></p><p class=MsoNormal style="margin-bottom:0cm;margin-bottom:.0001pt;line-height:normal"><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif">Has&#322;o jest wa&#380;ne 90 dni. Zalogowa&#263; si&#281; mo&#380;na na stronie <span class=MsoHyperlink><a href="https://portal.office.com">https://portal.office.com</a></span><o:p></o:p></span></p><p class=MsoNormal style="margin-bottom:0cm;margin-bottom:.0001pt;line-height:normal"><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif"><o:p>&nbsp;</o:p></span></p><p class=MsoNormal style="margin-bottom:0cm;margin-bottom:.0001pt;line-height:normal"><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif"><o:p>&nbsp;</o:p></span></p><p class=MsoNormal style="margin-bottom:0cm;margin-bottom:.0001pt;line-height:normal"><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif">Z harcerskim pozdrowieniem<o:p></o:p></span></p><p class=MsoNormal style="margin-bottom:0cm;margin-bottom:.0001pt;line-height:normal"><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif">Czuwaj<br><br><o:p></o:p></span></p><p style="margin:0cm;margin-bottom:.0001pt;background:white"><b><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif;color:#0B5394;border:none windowtext 1.0pt;padding:0cm;background:white">'+$global:PodpisAdministratora+'&nbsp;</span></b><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif;color:#0B5394;border:none windowtext 1.0pt;padding:0cm;background:white"><br>'+$global:FunkcjaAdministratora+'</span></div></body></html>'

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

