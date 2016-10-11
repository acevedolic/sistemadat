using System;
using System.Collections;
using System.Net;
using System.Runtime.InteropServices;

namespace CDatos
{
    [ComVisibleAttribute(true)]  //Deja la clase visible para COM
    [Guid("AA763944-F7B8-4401-B567-61BEFA299166")] //GUID que generamos, Identificador de la Libreria
    [ProgId("CDatos.Class")] //Identificador para poder Acceder a esta clase desde el exterior

    public class DUsuarios
    {
        private Conexion objConexion = new Conexion();
        private ArrayList arrayConvertidoDIN = new ArrayList();
        private string[] separadoDevueltoLocal = null;
        private string laIp;
        private string laboratorio;

        private string nombreUsuario = string.Empty;
        private string usuario = string.Empty;
        private string clave = string.Empty;
        private string encontrado = string.Empty;
        public bool saberEstadoCn()
        {
            try
            {
                System.Net.IPHostEntry host = System.Net.Dns.GetHostEntry("portal.utec.edu.sv");
                return true;

            }
            catch (Exception es)
            {

                return true;
            }
        }


        public DUsuarios()
        {

        }

        public DUsuarios(string usuario, string clave)
        {
            this.Usuario = usuario;
            this.Clave = clave;
        }

        public string NombreUsuario
        {
            get
            {
                return nombreUsuario;
            }

            set
            {
                nombreUsuario = value;
            }
        }

        public string Encontrado
        {
            get
            {
                return encontrado;
            }

            set
            {
                encontrado = value;
            }
        }

        public string Usuario
        {
            get
            {
                return usuario;
            }

            set
            {
                usuario = value;
            }
        }

        public string Clave
        {
            get
            {
                return clave;
            }

            set
            {
                clave = value;
            }
        }

        public string LaIp
        {
            get
            {
                return laIp;
            }

            set
            {
                laIp = value;
            }
        }

        public string Laboratorio
        {
            get
            {
                return laboratorio;
            }

            set
            {
                laboratorio = value;
            }
        }
        //este metodo retorna un JSON del webService de UTEC
        //pero solo retorna el tipo de usuario
        //en caso no lo encuentra en el servidor utec retorna -1
        public string BuscarDIN(DUsuarios Usuarios)
        {
            string cadenaDevueltaDin = string.Empty;
            cadenaDevueltaDin = objConexion.objDIN.Login_Lab(Usuarios.Usuario, Usuarios.Clave);
            char[] delimitadores = { ':', ',', '{', '}', '\"' };
            string[] valoresSeparados = cadenaDevueltaDin.Split(delimitadores);
            arrayConvertidoDIN.Clear();
            foreach (string valor in valoresSeparados)
            {
                if (valor.Trim() != "")
                {
                    arrayConvertidoDIN.Add(valor);
                }
            }
            Encontrado = arrayConvertidoDIN[1].ToString();
            return Encontrado;
        }
        //Este metodo retorna del webService de DAT 
        //en un array el nombre y tipo de usuario desde la base DAT
        //y solo retorna el nombre en caso lo encuetre de lo contrario retorna -1
        public string BuscarDAT(DUsuarios usuarios)
        {
            DEncriptar objEncriptar = new DEncriptar();

            string buscarClaveEncriptada = objEncriptar.encriptarClave(usuarios.Clave);

            referenciaDAT.consultas objDAT = new referenciaDAT.consultas();
            string devueltoLocal = objDAT.buscarLocal(usuarios.Usuario, buscarClaveEncriptada);

            char[] delimitadores2 = { ',' };
            separadoDevueltoLocal = devueltoLocal.Split(delimitadores2);
            Encontrado = separadoDevueltoLocal[0].ToString();
            return Encontrado;
        }
        public void asignarNombreUTEC()
        {
            //Este metodo asigna el nombre de usuario en caso lo encuntre en UTEC            
            NombreUsuario = arrayConvertidoDIN[3].ToString();
        }
        public void asignarNombreDAT()
        {
            //Este metodo asigna el nombre de usuario en caso lo encuntre en DAT            
            NombreUsuario = separadoDevueltoLocal[0].ToString();
        }
        //Este metodo retorna -1 en caso no esta activo en Practica Libre
        //de lo contrario retorna el numero de maquina en la que esta asignado
        public string estadoPl(DUsuarios usuario)
        {
            referenciaDAT.consultas objDAT = new referenciaDAT.consultas();
            string estado = objDAT.retornaEstadoPL(usuario.Usuario);
            return estado;
        }
        public string NombrePC()
        {
            string pc = Environment.MachineName;
            return pc;
        }
        public void ObtenetIpLaboratorio()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                }
            }
            LaIp = localIP;
            string[] lab = localIP.Split('.');
            Laboratorio = lab[2];
        }
        public string r_fecha()
        {
            string nFecha1 = "-1";
            try
            {
                char[] delimitadores = { ',' };

                referenciaDAT.consultas objDAT = new referenciaDAT.consultas();
                string fechaServidor = objDAT.retornaFechaHora("hola");

                string[] fechaSeparada = fechaServidor.Split(delimitadores);
                string m1, d1;
                if (int.Parse(fechaSeparada[1]) > 9)
                    m1 = fechaSeparada[1];
                else
                    m1 = "0" + fechaSeparada[1];
                if (int.Parse(fechaSeparada[2]) > 9)
                    d1 = fechaSeparada[2];
                else
                    d1 = "0" + fechaSeparada[2];
                nFecha1 = fechaSeparada[0] + "/" + m1 + "/" + d1 + " " + fechaSeparada[3];

            }
            catch (Exception)
            {

            }
            return nFecha1;
        }
        /*********************************************************************************/
        public string validar(DUsuarios usuarios)
        {

            int encontrado = -1;
            string nombre = "";
            string datos = "";
            string estado = "";

            //usuarios.Usuario = usuario;
            //usuarios.Clave = clave;

            Usuario = usuarios.Usuario;
            Clave = usuarios.Clave;

            //Busca en el servidor de la DIN
            if (BuscarDIN(usuarios) != "-1")
            {
                encontrado = 1;
            }
            //Busca en el servidor DAT
            else if (usuarios.BuscarDAT(usuarios) != "-1")
            {
                encontrado = 2;
            }
            //si no lo encuntra asigna -1
            else
            {
                encontrado = -1;
            }

            //Valida si lo encontro
            if (encontrado != -1)
            {
                //si lo encontro captura el nombre                              
                if (encontrado == 1)
                {
                    usuarios.asignarNombreUTEC();
                    nombre = usuarios.NombreUsuario;
                }
                else if (encontrado == 2)
                {
                    usuarios.asignarNombreDAT();
                    nombre = usuarios.Encontrado;
                }

                //busca si esta activo en un laboratorio
                //si retorna -1 significa que NO esta activo en un laboratorio
                //de lo contrario retorna el nombre de la pc donde esta activo el usuario

                estado = usuarios.estadoPl(usuarios);
                if (estado == "-1")
                {
                    datos = "0,";
                }
                else
                {
                    datos = "-1," + estado + ",";
                }
                usuarios.ObtenetIpLaboratorio();
                datos += nombre + "," + usuarios.NombrePC();
            }
            else
            {
                datos = "2,Error... Usuario no existe.";
            }
            string validar = datos.Substring(0, 1).ToString();
            if (validar == "0")
            {
                try
                {
                    referenciaDAT.consultas objDatIn = new referenciaDAT.consultas();
                    objDatIn.insertarDetallePL(usuarios.Usuario, r_fecha(), r_fecha(), Environment.MachineName, LaIp.ToString(), Laboratorio.ToString());
                    datos = "1,Datos insertados," + Laboratorio.ToString();
                }
                catch (Exception e)
                {
                    datos = "Error Insertar Detalle, " + e.Message;
                }

            }
            return datos;
        }
        /*********************************************************************************/

        /*********************************************************************************/
        public string validarCierreForm(string contraseña)
        {
            string estado = "";
            //Console.WriteLine(contraseña);
            referenciaDAT.consultas objDatIn = new referenciaDAT.consultas();
            DEncriptar objEncripta = new DEncriptar();
            string encriptada = objEncripta.encriptarClave(contraseña);
            ObtenetIpLaboratorio();
            estado = objDatIn.retornaClaveBloqueo(encriptada, Laboratorio);
            return estado;
        }
        /*********************************************************************************/
        public void cerrarSesion(DUsuarios pUsuario)
        {
            referenciaDAT.consultas objDatIn = new referenciaDAT.consultas();
            objDatIn.actualizarEstadoDetallePL(pUsuario.Usuario);            
        }
    }
}
C P A N E G O C I O S  
 