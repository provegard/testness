# From http://devio.wordpress.com/2009/09/15/command-line-xslt-processor-with-powershell/
# Modified to enable script execution.

param ($xml, $xsl, $output)

if (-not $xml -or -not $xsl -or -not $output)
{
	Write-Host "& .\xslt.ps1 [-xml] xml-input [-xsl] xsl-input [-output] transform-output"
	exit;
}

trap [Exception]
{
	Write-Host $_.Exception;
}

$settings = New-Object System.Xml.Xsl.XsltSettings;
$settings.EnableScript = $true;

$xslt = New-Object System.Xml.Xsl.XslCompiledTransform;
$xslt.Load($xsl, $settings, $null);
$xslt.Transform($xml, $output);

Write-Host "generated" $output;