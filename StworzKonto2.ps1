<#

.SYNOPSIS
To jest skrypt pomagajacy zarzadzajacy kontami w O365

.DESCRIPTION
To jest skrypt edytujacy zarzadzajacy kontami w O365 na bazie wskazanego skoroszytu, 
w którym pierwszy arkusz o nazwie Konta zawiera nastepujace kolumny:
Imie	Nazwisko	Email prywatny	Email zhp	Hufiec	Hasło


#>
<#
Copyright 2019 Andrzej Żmijewski

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#>

Param(
    [Parameter(HelpMessage = 'Sciezka do arkusza z danymi uzytkownikow')] [string]$Path,
    [Parameter(HelpMessage = 'Sciazka do pliku z haslem dla aplikacji do poczty O365')][string]$MailPassFile = "C:\Users\andrz\MailPassword.txt",
    [Parameter(HelpMessage = 'Parsuje wszsytkie rekordy na raz')][switch]$All = $false,
    [Parameter(HelpMessage = 'Definiuje kto ma byc w polu DoWiadomosci')][string]$KopiaDo = "aktywacja@gdanska.zhp.pl"
)
 
[System.IO.Path]::HasExtension($Path)


#Zmienne globalne
$commands="Znajdź konta dla nazwiska","Znajdź konto dla emila","Reset hasła","Stwórz konto","Dodaj Licencje","Wyjdź"
$global:cc= $KopiaDo
$global:PodpisAdministratora="phm.&nbsp;Andrzej  &#379;mijewski"  #Polskie znaki: http://hektor.umcs.lublin.pl/~awmarczx/awm/info/pl-codes.htm
$global:FunkcjaAdministratora="Cz&#322;onek zespo&#322;u ds. informacji&nbsp;Chor&#261;gwi Gda&#324;skiej ZHP"

<#wygenerowany uzywajac komend (trzymaj go w prywatnej przestrzeni i nie udostepniaj):
$Secure = Read-Host -AsSecureString #Podaj unikalne haslo wygenerowane dla aplikacji w opjach zabezpieczen O365
$Encrypted = ConvertFrom-SecureString -SecureString $Secure
Set-Content -Path $global:mailpassfile  -Value $Encrypted
#>
$global:mailpassfile = $MailPassFile
$global:username = "andrzej.zmijewski@zhp.net.pl"

$global:file = $Path
$global:debug = $False


function GetMailCretentials {
    #genreted using command:
    #$Secure = Read-Host -AsSecureString
    #$Encrypted = ConvertFrom-SecureString -SecureString $Secure
    #Set-Content -Path $global:mailpassfile  -Value $Encrypted

    $SecureStringPassword = Get-Content -Path $global:mailpassfile  | ConvertTo-SecureString

    $EmailCredential = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $global:username,$SecureStringPassword
    return  $EmailCredential
    
}

function SendMailWithPassword ($konto, $haslo){
    $mailcredentials = GetMailCretentials

    $mail = $konto.'Email zhp'
    $From = $global:username
    $To = $konto.'Email priv'
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
<b><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif;color:#0B5394"> '+$haslo+'</span></b><b><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif"><o:p></o:p></span></b></p><p class=MsoNormal style="margin-bottom:0cm;margin-bottom:.0001pt;line-height:normal"><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif"><o:p>&nbsp;</o:p></span></p><p class=MsoNormal style="margin-bottom:0cm;margin-bottom:.0001pt;line-height:normal"><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif">Has&#322;o jest wa&#380;ne 90 dni. Zalogowa&#263; si&#281; mo&#380;na na stronie <span class=MsoHyperlink><a href="https://portal.office.com">https://portal.office.com</a></span><o:p></o:p></span></p><p class=MsoNormal style="margin-bottom:0cm;margin-bottom:.0001pt;line-height:normal"><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif"><o:p>&nbsp;</o:p></span></p><p class=MsoNormal style="margin-bottom:0cm;margin-bottom:.0001pt;line-height:normal"><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif"><o:p>&nbsp;</o:p></span></p><p class=MsoNormal style="margin-bottom:0cm;margin-bottom:.0001pt;line-height:normal"><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif">Z harcerskim pozdrowieniem<o:p></o:p></span></p><p class=MsoNormal style="margin-bottom:0cm;margin-bottom:.0001pt;line-height:normal"><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif">Czuwaj<br><br><o:p></o:p></span></p><p style="margin:0cm;margin-bottom:.0001pt;background:white"><b><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif;color:#0B5394;border:none windowtext 1.0pt;padding:0cm;background:white">'+$global:PodpisAdministratora+'&nbsp;</span></b><span lang=PL style="font-size:10.0pt;font-family:"Trebuchet MS",sans-serif;color:#0B5394;border:none windowtext 1.0pt;padding:0cm;background:white"><br>'+$global:FunkcjaAdministratora+'</span></div></body></html>'

$SMTPServer = "smtp.office365.com"
$SMTPPort = "587"
Send-MailMessage -Encoding UTF8 -From $From -to $To -Cc $global:cc -Subject $Subject -Body $Body -BodyAsHtml -SmtpServer $SMTPServer -Port $SMTPPort -UseSsl -Credential $mailcredentials
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
        if ($global:isAdmin)
        {
            Install-Module -Name ImportExcel
        }
        else {
            Import-Module -Name ImportExcel
        }
    
        $FileBrowser = New-Object System.Windows.Forms.OpenFileDialog 
        $null = $FileBrowser.ShowDialog()
        $global:file = $FileBrowser.FileName 
    }
    
    $stores = Import-Excel -WorksheetName Konta $global:file
    return $stores
}

function AskForCommands($commands)
{
    #Write-Host "Wybierz komendę: "
    $commands | foreach {$i = 1} { Write-Host "$i -  $_"; $i++ }
    return [int](getKeyResponse "Wybierz komendę ") - 49
    #return $HOST.UI.RawUI.ReadKey().VirtualKeyCode - 49
}

function ZapiszHasla($hasla)
{
    # Create Excel COM Object
    $excel = New-Object -ComObject Excel.Application
 
    # Make the Excel windows visible
    $excel.Visible = $true
 
    # Add Workbook
    $workbook = $excel.Workbooks.Open($global:file)
 
    if (not $workbook) 
    {
        throw "Nie udalo sie otworzyc wskazanego arkusza"
    }

    # Add Worksheet
    $sheet1 = $workbook.worksheets.item(1)
    if ("$sheet1.Name" -eq "Konta") 
    { throw "Zla nazwa arkusza. Oczekuje, ze pierwszy arkusz to 'Konta'" }
    else
    {
        Write-Output "Przetwarzam: $file"
    }

    if (-not $sheet1.cells.Item(1,6).Text -eq "Hasło")
    {
        throw "Zły format arkusza. Brak Kolumny haslo na 6 pozycji"
    }
    $hasla | foreach {$linia = 2}{ $sheet1.cells.Item($linia,6).Text = $_; $linia++}

    # Save the workbook
    $workbook.SaveAs($file)
 
    # Close the workbook
    $workbook.Close($true)
 
    # Quit Excel
    $excel.Quit()
 
    # Release COM Object
    [void][Runtime.Interopservices.Marshal]::ReleaseComObject($excel)
}

function ZnajdzKontaDlaNazwiska($konto)
{
    Get-MsolUser -SearchString $konto.Nazwisko | select FirstName, LastName, Office, SignInName | Sort FirstName
}

function ZnajdźKontoDlaEmila($konto) {
        
    Get-MsolUser -UserPrincipalName $konto."Email zhp"
}

function ResetHasla ($konto) {
    $password = $konto."Hasło" 
    #if ("$konto.'Hasło'" -eq "")
    #{
        $length = 10 ## characters
        $nonAlphaChars = 3
        $password = [System.Web.Security.Membership]::GeneratePassword($length, $nonAlphaChars)
        Write-Host "Brak Hasla dla $($konto."Email zhp"). Nowe haslo: $password"
    #}
    Write-Host "Resetuje haslo : $password"
    Set-MsolUserPassword -UserPrincipalName $konto."Email zhp" -NewPassword $password -ForceChangePassword $True
    SendMailWithPassword $konto $password
}

function AddLicense ($konto) {
    Set-MsolUserLicense -UserPrincipalName -UserPrincipalName $konto."Email zhp" AddLicenses 'gkzhp:STANDARDWOFFPACK','gkzhp:POWER_BI_STANDARD'
}


function StworzKonto ($konto) {
    Write-Host "Zakladam konto $konto"
    $uzytokownik = New-MsolUser -UserPrincipalName $konto."Email zhp" -FirstName $konto."Imie" -LastName $konto."Nazwisko" -DisplayName "$($konto."Imie") $($konto."Nazwisko")" -Office $konto."Hufiec" -LicenseAssignment 'gkzhp:STANDARDWOFFPACK','gkzhp:POWER_BI_STANDARD' -UsageLocation PL
    SendMailWithPassword $konto $uzytokownik.Password
    return $uzytokownik
}

##################################################

$currentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
$global:isAdmin = $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)


#$global:credentials = Get-Credential $global:username

Import-Module MsOnline
Connect-MsolService #-Credential $credentials


$konta = LoadData               
Write-Output $konta
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
        $runForAll = AskConfirmation "Wykonac komende dla wszykich kont?"
    }

    $konta | ForEach-Object  {
        Write-Host "Proceduje: $($_.'Email zhp')"
        $shouldRun = $True
        if (-not $runForAll) { $shouldRun = ( AskConfirmation "Czy wykonac komende dla $($_.'Email zhp')" ) }
        if ($shouldRun)
        {
            if ($commands[$command] -eq "Znajdź konta dla nazwiska") {ZnajdzKontaDlaNazwiska $_}
            elseif($commands[$command] -eq "Znajdź konto dla emila") {ZnajdźKontoDlaEmila $_}
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

