This document describes how to use the webMVCTest application and test an MVN website

# Command line #
When you build the application it produces a console application. Use it the following way:

WebMVNTest.exe [-r] [-l] [-d] [-t 

&lt;testSet&gt;

] fileName.xml

Options are:
  * -r report the results in a PDF
  * -d add debugging output
  * -l add logging output to file
  * -t <testSet] test only a specific testSet. Useful when building your test project.

# XML Structure #


&lt;project&gt;


> 

&lt;name&gt;


> 

&lt;description&gt;


> 

&lt;baseUrl&gt;


> 

&lt;headers&gt;


> > 

&lt;header key="" value=""&gt;

 (See [supported headers](SupportedHeaders.md) for more details)

> 

&lt;functions&gt;


> > 

&lt;function&gt;



> 

&lt;testSets&gt;


> > 

&lt;testSet&gt;


> > > 

&lt;name&gt;


> > > 

&lt;description&gt;


> > > 

&lt;function&gt;



Function has the following structure:


&lt;function&gt;



> 

&lt;name&gt;


> 

&lt;description&gt;


> 

&lt;url&gt;

 (the baseUrl of the project is added to this url)
> 

&lt;method&gt;

 (either POST or GET)
> 

&lt;headers&gt;


> > 

&lt;header key="" value=""&gt;

 (See [supported headers](SupportedHeaders.md) for more details)

> 

&lt;params&gt;


> > 

&lt;param key="" value=""&gt;

 (params are only used in POSTs)

> 

&lt;postbody&gt;

 (only used in POSTs, can handle CDATA)
> 

&lt;assertions&gt;


> > 

&lt;notNull&gt;

 (checks if the call returns content)
> > 

&lt;responseTextDoesNotContain value=""&gt;

 (checks if the response text does not contain a certain text)
> > 

&lt;responseTextContains value=""&gt;

 (checks if the response text does contain a certain text)
> > 

&lt;responseTextLargerThan size=""&gt;

 (checks if the content length is larger than ..)
> > 

&lt;responseCodeEquals statusCode=""&gt;

 (check the statusCode of the response; 200,302,500 etc.)
> > 

&lt;jsonArraySizeLargerThan size=""&gt;

 (check if the result is a json array and larger than a given size)
> > 

&lt;jsonArrayValueEquals row="" column="" value=""&gt;

 (check if a certain column in a row of json objects is equal to a certain value)

> 

&lt;processors&gt;


> > 

&lt;jsonArray row="" column="" var=""&gt;

 (defines a new variable containing the value located in the json array response at the given row and column)
> > 

&lt;jsonObject column="" var=""&gt;

 (defines a new variable containing the value located in the json object response at with the given column name)