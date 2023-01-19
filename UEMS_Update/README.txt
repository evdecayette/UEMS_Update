GridView and BootStrap

ASPX page gridview object:
    AllowPaging="False" 
    class='table table-striped table-bordered table-hover'
    AllowCustomPaging="False" >

    <script>
        $(document).ready(function () {
            $('#ContentPlaceHolder1_gridview1').DataTable({
                responsive: true
            });
        });
    </script>

CS page:
    //Place after gridview1.DataBind()

    // Section importante pour BootStrap
    if (gridview1.Rows.Count > 0)
    {
        //Adds THEAD and TBODY Section.
        gridview1.HeaderRow.TableSection = TableRowSection.TableHeader;

        //Adds TH element in Header Row.  
        gridview1.UseAccessibleHeader = true;

        //Adds TFOOT section. 
        gridview1.FooterRow.TableSection = TableRowSection.TableFooter;
    }


==========================================================================================================

Below goes the consolidated table with all the differences as discussed at the top.
 		        Server.Transfer		                            Response.Redirect	
Redirection		Redirection is done by the server.		        Redirection is done by the browser client.	
Browser URL		Does not change.		                        Changes to the redirected target page.	
When to use		Redirect between pages of the same server.		Redirect between pages on different server and domain.	

What is the Importance of “preserveForm” Flag in “Server.Transfer”?
“Server.Transfer” helps to redirect from one page to other page. If you wish to pass query string and form data of the first page to the target page during this redirection, you need to set “preserveForm” to “true” as shown in the below code.

Server.Transfer("Webform2.aspx",true);

By default, the value of “preserveForm” is “true”.
Response.Redirect(URL,true) vsResponse.Redirect(URL,false) ?

Response.Redirect(URL,false): Client is redirected to a new page and the current page on the server will keep processing ahead.

Response.Redirect(URL,true): Client is redirected to a new page but the processing of the current page is aborted.

==========================================================================================================

