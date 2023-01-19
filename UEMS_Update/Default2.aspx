<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default2.aspx.cs" Inherits="Default2" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head id="Head1" runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="description" content="" />
    <meta name="author" content="" />

    <title>UEspoir Système de Gestion</title>

    <link href="css/formItems.css" rel="stylesheet" />
    <!-- Bootstrap Core CSS -->
    <link href="vendor/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <!-- MetisMenu CSS -->
    <link href="vendor/metisMenu/metisMenu.min.css" rel="stylesheet" />
    <!-- Custom CSS -->
    <link href="dist/css/sb-admin-2.css" rel="stylesheet"/> 

    <link href="http://www.jqueryscript.net/css/jquerysctipttop.css" rel="stylesheet" type="text/css"/>
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.11.1/jquery.min.js"></script>
    <script src="http://netdna.bootstrapcdn.com/bootstrap/3.1.1/js/bootstrap.min.js"></script>
    <script src="editor/editor.js"></script>

    <link href="css/font-awesome.min.css" rel="stylesheet"/>
    <link href="editor/editor.css" type="text/css" rel="stylesheet"/>

    <script type="text/javascript">
        $(document).ready(function () {
            $("#txtEditor").Editor();
        });
      </script>
</head>

<body>
        <div id="wrapper">
        <!-- Navigation -->
        <nav class="navbar navbar-default navbar-static-top" role="navigation" style="margin-bottom: 0">
            <div class="navbar-header">
                <a class="navbar-brand" href="Default.aspx">Site Administratif de L'Université Espoir</a>
            </div>
            <!-- /.navbar-header -->

            <ul class="nav navbar-top-links navbar-right">
                <li class="dropdown">
                    <a class="dropdown-toggle" data-toggle="dropdown" href="#">
                        <i class="fa fa-envelope fa-fw"></i> <i class="fa fa-caret-down"></i>
                    </a>
                    <ul class="dropdown-menu dropdown-messages">
                        <li>
                            <a href="CourrieldeMasseEtudiants.aspx">
                                <div>
                                    <strong>Courriel de Masse</strong>
                                    <span class="pull-right text-muted">
                                        <em>Etudiants</em>
                                    </span>
                                </div>
                            </a>
                        </li>
                        <li class="divider"></li>
                        <li>
                            <a href="CourrieldeMasseProfesseurs.aspx">
                                <div>
                                    <strong>Courriel de Masse</strong>
                                    <span class="pull-right text-muted">
                                        <em>Professeurs</em>
                                    </span>
                                </div>
                            </a>
                        </li>
                    </ul>
                    <!-- /.dropdown-messages -->
                </li>
                <!-- /.dropdown -->
                <li class="dropdown">
                    <a class="dropdown-toggle" data-toggle="dropdown" href="#">
                        <i class="fa fa-tasks fa-fw"></i> <i class="fa fa-caret-down"></i>
                    </a>
                    <ul class="dropdown-menu dropdown-tasks">
                        <li>
                            <a href="#">
                                <div>
                                    <p>
                                        <strong>Task 1</strong>
                                        <span class="pull-right text-muted">40% Complete</span>
                                    </p>
                                    <div class="progress progress-striped active">
                                        <div class="progress-bar progress-bar-success" role="progressbar" aria-valuenow="40" aria-valuemin="0" aria-valuemax="100" style="width: 40%">
                                            <span class="sr-only">40% Complete (success)</span>
                                        </div>
                                    </div>
                                </div>
                            </a>
                        </li>
                        <li class="divider"></li>
                        <li>
                            <a class="text-center" href="ToutesLesTaches.aspx">
                                <strong>See All Tasks</strong>
                                <i class="fa fa-angle-right"></i>
                            </a>
                        </li>
                    </ul>
                    <!-- /.dropdown-tasks -->
                </li>
                <!-- /.dropdown -->
                <li class="dropdown">
                    <a class="dropdown-toggle" data-toggle="dropdown" href="#">
                        <i class="fa fa-bell fa-fw"></i> <i class="fa fa-caret-down"></i>
                    </a>
                    <ul class="dropdown-menu dropdown-alerts">
                        <li>
                            <a href="#">
                                <div>
                                    <i class="fa fa-comment fa-fw"></i> New Comment
                                    <span class="pull-right text-muted small">4 minutes ago</span>
                                </div>
                            </a>
                        </li>
                        <li class="divider"></li>
                        <li>
                            <a class="text-center" href="TousLesCommentaires.aspx">
                                <strong>See All Alerts</strong>
                                <i class="fa fa-angle-right"></i>
                            </a>
                        </li>
                    </ul>
                    <!-- /.dropdown-alerts -->
                </li>
                <!-- /.dropdown -->
                <li class="dropdown">
                    <a class="dropdown-toggle" data-toggle="dropdown" href="#">
                        <i class="fa fa-user fa-fw"></i> <i class="fa fa-caret-down"></i>
                    </a>
                    <ul class="dropdown-menu dropdown-user">
                        <li><a href="#"><i class="fa fa-user fa-fw"></i> User Profile</a>
                        </li>
                        <li><a href="#"><i class="fa fa-gear fa-fw"></i> Settings</a>
                        </li>
                    </ul>
                    <!-- /.dropdown-user -->
                </li>
                <!-- /.dropdown -->
            </ul>
            <!-- /.navbar-top-links -->

            <div class="navbar-default sidebar" role="navigation">
                <div class="sidebar-nav navbar-collapse">
                    <ul class="nav" id="side-menu">
                        <li>
                            <a href="default.aspx"><i class="fa fa-dashboard fa-fw"></i>Page de Bienvenue</a>
                        </li>
                        <li>
                            <a href="#"><i class="fa fa-bar-chart-o fa-fw"></i>Nouveaux Etudiants<span class="fa arrow"></span></a>
                            <ul class="nav nav-second-level">
                                <li>
                                    <a href="Inscription.aspx">Nouvelles Inscriptions</a>
                                </li>
                                <li>
                                    <a href="ReussiteEchec.aspx">Notes Examen Français</a>
                                </li>
                                <li>
                                    <a href="ReussiteEchecMath.aspx">Notes Examen Classement Math</a>
                                </li>
                                <li>
                                    <a href="ReussiteEchecAnglais.aspx">Notes Examen Classement Anglais</a>
                                </li>
                            </ul>
                            <!-- /.nav-second-level -->
                        </li>
                        <li>
                            <a href="ListeEtudiants.aspx"><i class="fa fa-table fa-fw"></i>Liste de Tous les Etudiants</a>
                        </li>
                        <li>
                            <a href="#"><i class="fa fa-wrench fa-fw"></i>Etudiants<span class="fa arrow"></span></a>
                            <ul class="nav nav-second-level">
                                <li>
                                    <a href="MettreGroupeDansClasse.aspx">Enregistrer un Groupe</a>
                                </li>
                                <li>
                                    <a href="EntrerNotesPourGroupe.aspx">Entrer Notes Pour un Cours</a>
                                </li>
                            </ul>
                            <!-- /.nav-second-level -->
                        </li>
                        <li>
                            <a href="#"><i class="fa fa-wrench fa-fw"></i>Rapports<span class="fa arrow"></span></a>
                            <ul class="nav nav-second-level">
                                <li>
                                    <a href="RapportEntrees.aspx">Rapports Spéciaux</a>
                                </li> 
                                <li>
                                    <a href="ListeEtudiantsParCours.aspx">Liste des Etudiants Par Cours</a>
                                </li> 
                                <li>
                                    <a href="NombreEtudiantsParClasse.aspx">Nombre d'Etudiants Par Cours</a>
                                </li>                                                                                                                 
                                <li>
                                    <a href="ListeEtudiantsParDiscipline.aspx">Etudiants Par Discipline</a>
                                </li>
                            </ul>
                            <!-- /.nav-second-level -->
                        </li>
                        <li>
                            <a href="#"><i class="fa fa-wrench fa-fw"></i>Superviseur<span class="fa arrow"></span></a>
                            <ul class="nav nav-second-level">
                                <li>
                                    <a href="AddEditNotePassage.aspx">Changer Note de Passage</a>
                                </li>
                                <li>
                                    <a href="AddEditCours.aspx">Ajouter ou Editer un Cours</a>
                                </li>
                                <li>
                                    <a href="AddEditObligations.aspx">Ajouter ou Editer la Description d'une Obligation</a>
                                </li>
                                <li>
                                    <a href="ChoisirCoursPourSession.aspx">Offrir Cours (Session Courante)</a>
                                </li>
                                <li>
                                    <a href="SetupSession.aspx">Setup Nouvelle Session</a>
                                </li>
                            </ul>
                            <!-- /.nav-second-level -->
                        </li>                                           
                    </ul>
                </div>
                <!-- /.sidebar-collapse -->
            </div>
            <!-- /.navbar-static-side -->
        </nav>
        <div id="page-wrapper">
            <form id="particularForm" runat="server">

     
            <div class="container-fluid">  
            <div class="container">
            <div class='row'>
                <div class='col-lg-12'>
                    <div class='panel panel-default'>
                        <div class='panel-heading'>Cliquez -Toutes les Classes- ou Choisissez une Classe Spécifique</div>
                        <div class='panel-body'>
                            <div class="row">
                                <div class="col-lg-4">
                                    
                                        <asp:CheckBox ID="chkToutesLesClasses" runat="server" Text="Toutes Les Classes" OnCheckedChanged="chkToutesLesClasses_CheckedChanged" />
                                    <div class="checkbox">
  <label><input type="checkbox" value="">Option 1</label>
</div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-5">
                                   
                                        <asp:Button ID="btnEnvoyer" runat="server" Width="140" Text="Envoyer Email" class="btn btn-outline btn-primary"/>
                                  
                                </div>
                                <div class="col-lg-7">
                                    
                                        <asp:Button ID="btnCancel" runat="server" Width="140" Text="Cancel" class="btn btn-outline btn-default" />  
                                                      
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6">  
                                    <div class="dynamiSelect">                                   
                                      <asp:DropDownList ID="ddlCoursOfferts" runat="server" DataValueField="CoursOffertID" DataTextField="Description" AutoPostBack="True" OnSelectedIndexChanged="ddlCoursOfferts_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-6">
                                    
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-5">                                             
                                        <asp:GridView ID="gvStudents" runat="server" DataKeyNames="PersonneID" AutoGenerateColumns="False" 
                                            ShowFooter="false" AllowPaging="true" class='table table-striped table-bordered table-hover'
                                            EnablePersistedSelection="True" ShowHeaderWhenEmpty="True" width='100%' >                           
                                            <Columns>
                                                <asp:TemplateField  HeaderText="Nom Complet">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblNomComplet" runat="server" Text='<%# Bind("NomComplet") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField  HeaderText="Email">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEmail" runat="server" Text='<%# Bind("Email") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <HeaderStyle BackColor="#003300" Font-Bold="True" Font-Size="Medium" ForeColor="White" />
                                        </asp:GridView>                                   
                                </div>
                                <div class="col-lg-1 nopadding">
                                </div>
                                <div class="col-lg-6 nopadding">
    	                            <div id="txtEditor"></div> 
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-12">                                   
                                        <asp:Label ID="lblError" runat="server" Font-Bold="True" ForeColor="#FF3300"></asp:Label>                                     
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            </div>
            </div> 



    </form>

    </div>   

        </div>




</body>
</html>

