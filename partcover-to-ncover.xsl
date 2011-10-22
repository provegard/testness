<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:user="urn:my-scripts">

  <msxsl:script language="C#" implements-prefix="user">
    <![CDATA[
     public string filename(string path){
       return (new System.IO.FileInfo(path)).Name;
     }
      ]]>
  </msxsl:script>
  <msxsl:script language="C#" implements-prefix="user">
    <![CDATA[
     public string identity(string path){
       if (System.IO.File.Exists(path))
         try { return System.Reflection.Assembly.LoadFile(path).FullName; }  catch (Exception) {}
       return "?";
     }
      ]]>
  </msxsl:script>

  <xsl:output method="xml" />

  <xsl:template match="PartCoverReport">
    <coverage profilerVersion="{@version}" driverVersion="{@version}" startTime="{@date}" measureTime="{@date}">
      <xsl:for-each select="//Assembly">
        <xsl:variable name="assemblyRef" select="@id" />
        <module moduleId="{@id}" assembly="{@name}">
          <xsl:attribute name="name">
            <xsl:value-of select="user:filename(@module)"/>
          </xsl:attribute>
          <xsl:attribute name="assemblyIdentity">
            <xsl:value-of select="user:identity(@module)"/>
          </xsl:attribute>
          <xsl:for-each select="//Method">
            <xsl:choose>
              <xsl:when test="../@asmref = $assemblyRef">
                <method name="{@name}" class ="{../@name}" sig="{@sig}" excluded="false" instrumented="true" metadataToken="{@methoddef}">
                  <xsl:for-each select="./pt">
                    <xsl:choose>
                      <xsl:when test="@sl != ''">
                        <xsl:variable name="fileRef" select="@fid" />
                        <xsl:variable name="file" select="//File[@id = $fileRef]/@url" />
                        <seqpnt visitcount="{@visit}" line="{@sl}" column="{@sc}" endline="{@el}" endcolumn="{@ec}" excluded="false">
                          <xsl:attribute name="document">
                            <xsl:value-of select="$file"/>
                          </xsl:attribute>
                        </seqpnt>
                      </xsl:when>
                    </xsl:choose>
                  </xsl:for-each>
                </method>
              </xsl:when>
            </xsl:choose>
          </xsl:for-each>
        </module>
      </xsl:for-each>
    </coverage>
  </xsl:template>
</xsl:stylesheet>