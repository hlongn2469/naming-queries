<%@ Page Title="NamesQuery" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Async="true" Inherits="namesquery._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="col-md-4">
        <h1> This application enables loading data from Blob storage to CosmosDb, deleting data from Blob and CosmosDb, and looking for names (Lastname-firstname) in CosmosDB </h1>
    </div>

    <div class="col-md-4">
        <h1 style="color: #FF0000; background-color: #FFFFFF">Clear</h1>
        <asp:Button ID="Button2" runat="server" OnClick="deleteButton" Text="Clear Names" Height="25px" Width="140px" />
        <p id="DeleteMessage" runat="server"></p>
        <p id="DeleteErrorMessage" runat="server"></p>
        <br />
        <br />
    </div>

    <div class="col-md-4">
        <h1 style="color: #00FF00">Load</h1>
        <asp:Button ID="Button1" runat="server" OnClick="loadButton" Text="Load Names" Height="25px" Width="140px" />
        <br />
        <br />
        <br />
        <p id="LoadMessage" runat="server"></p>
        <p id="LoadErrorMessage" runat="server"></p>
        <br />
        <textarea id="TextArea1" runat="server" name="S1"></textarea>
        <br />
        <br />
        <br />
    </div>

    <div class="col-md-4">
        <h1 style="color: #0033CC">Query</h1>
        <p runat="server">Last Name:</p>
        <input id="LastNameField" type="text" runat="server"/>
        <br />
        <br />
        <p runat="server">First Name:</p>
        <input id="FirstNameField" type="text" runat="server"/>
        <br />
        <asp:Button ID="Button3" runat="server" OnClick="queryButton" Text="Look up names" Height="25px" Width="140px" />
        <p id="QueryMessage" runat="server"></p>
        <p id="QueryErrorMessage" runat="server"></p>
        <br />
        <p>Query Result </p>
        <textarea id="TextArea2" runat="server" name="S2"></textarea>
        
    </div>
</asp:Content>
