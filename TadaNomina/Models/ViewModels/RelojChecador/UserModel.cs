using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TadaNomina.Models.ViewModels.RelojChecador
{
    public class UserModel
    {
        //***********************************User*************************************//
        //Disable~OAuth

        //Peticion
        [Display(Name = "Numero IMSS")]
        public string Identifier { get; set; } //Required

        [Display(Name = "Correo")]
        public string Email { get; set; } //Required

        //Respuesta
        //Si la ejecución es correcta retorna el mensaje “OK”. En caso contrario retorna “WARNING:user didn´t exist”

        /////////////////////////////////////////////////////////////////////////////////////
        //Enable~OAuth

        //Peticion
        //public string Identifier {get;set;} //Required || Disable-Peticion
        // public string Email {get;set;} //Required || Disable-Peticion

        //Respuesta
        //Si la ejecución es correcta retorna el mensaje “OK”. En caso contrario retorna “ERROR:there is a user active with the same identifiers”

        /////////////////////////////////////////////////////////////////////////////////////
        //Add~OAuth

        //Peticion
        //public string Identifier {get;set;} //Required || Disable-Peticion
        [Display(Name = "Nombre")]
        public string Name { get; set; } //Required

        [Display(Name = "Apellidos")]
        public string LastName { get; set; } //Required
                                             //public string Email {get;set;} //Required || Disable-Peticion
        public string Adress { get; set; }
        public string Phone { get; set; }
        public int Enabled { get; set; }
        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
        public string GroupIdentifier { get; set; }
        public string ContractDate { get; set; }
        public string userCompanyIdentifier { get; set; }
        public string weeklyHoursCode { get; set; }
        public string endContractDate { get; set; }
        public string positionIdentifier { get; set; }
        public string integrationCode { get; set; }
        public string userCard { get; set; }

        //Respuesta
        //Si la ejecución es correcta retorna el mensaje “OK”. En caso contrario retorna “ERROR:there is a user active with the same identifiers”

        /////////////////////////////////////////////////////////////////////////////////////
        //Edit~OAuth

        //Peticion
        //public string Identifier {get;set;} //Required || Disable-Peticion
        //public string Name {get;set;} //Required || Add-Peticion
        //public string LastName {get;set;} //Required || Add-Peticion
        //public string Email {get;set;} //Required || Disable-Peticion
        //public string Adress {get;set;} //Add-Peticion
        //public string Phone {get;set;} //Add-Peticion
        //public int Enabled {get;set;} //Add-Peticion
        //public string Custom1 {get;set;} //Add-Peticion
        //public string Custom2 {get;set;} //Add-Peticion
        //public string Custom3 {get;set;} //Add-Peticion
        //public string GroupIdentifier {get;set;} //Add-Peticion
        //public string ContractDate {get;set;} //Add-Peticion
        //public string userCompanyIdentifier {get;set;} //Add-Peticion
        //public string weeklyHoursCode {get;set;} //Add-Peticion
        //public string endContractDate {get;set;} //Add-Peticion
        //public string positionIdentifier {get;set;} //Add-Peticion
        //public string integrationCode {get;set;} //Add-Peticion
        //public string userCard {get;set;} //Add-Peticion
        public int? legalSundayIndicator { get; set; }

        //Respuesta
        //Si la ejecución es correcta retorna el mensaje “OK”. En caso contrario retorna un mensaje de error.

        /////////////////////////////////////////////////////////////////////////////////////
        //List~OAuth

        //Peticion
        //La petición no contiene parámetros de entrada:

        //Respuesta
        //public string Identifier {get;set;} //Disable-Peticion
        //public string Name {get;set;} //Add-Peticion
        //public string LastName {get;set;} //Add-Peticion
        //public string Email {get;set;} //Disable-Peticion
        //public string Adress {get;set;} //Add-Peticion
        //public string Phone {get;set;} //Add-Peticion
        //public int Enabled {get;set;} //Add-Peticion
        //public string Custom1 {get;set;} //Add-Peticion
        //public string Custom2 {get;set;} //Add-Peticion
        //public string Custom3 {get;set;} //Add-Peticion
        //public string GroupIdentifier {get;set;} //Add-Peticion
        public string GroupDescription { get; set; }
        //public string ContractDate {get;set;} //Add-Peticion
        public string UserProfile { get; set; }
        //public string userCompanyIdentifier {get;set;} //Add-Peticion
        //public string weeklyHoursCode {get;set;} //Add-Peticion
        //public string endContractDate {get;set;} //Add-Peticion
        //public string positionIdentifier {get;set;} //Add-Peticion
        public string positionName { get; set; }
        //public string integrationCode {get;set;} //Add-Peticion
        //public string userCard {get;set;} //Add-Peticion
        //public int legalSundayIndicator {get;set;} //Edit-Peticion
        public string legacyHash { get; set; }

        /////////////////////////////////////////////////////////////////////////////////////
        //ProfileList~OAuth

        //Petición
        //La petición no contiene parámetros de entrada.

        //Respuesta
        public string NOMBRE_PERFIL { get; set; }
        //public string Identifier {get;set;} //Disable-Peticion

        /////////////////////////////////////////////////////////////////////////////////////
        //editProfile~OAuth
        //Peticion
        //public string Identifier {get;set;} //Required || Disable-Peticion
        //public string UserProfile {get;set;} //Required || List-Respuesta

        //Respuesta
        //Si la ejecución es correcta retorna el mensaje “OK”. En caso contrario retorna un mensaje de error.

        /////////////////////////////////////////////////////////////////////////////////////
        //AttendanceBook
        public List<LAPlannedIntervalModel> PlannedInterval { get; set; }
        //public string Identifier {get;set;} //Disable-Peticion
        //public string Name {get;set;} //Add-Peticion
        //public string LastName {get;set;} //Add-Peticion
        //public int Enabled {get;set;} //Add-Peticion
        //public string GroupDescription {get;set;} // List-Respuesta
        //public string Email {get;set;} //Disable-Peticion
    }
}