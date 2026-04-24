using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAdmin.Models;

/// <summary>
/// Clase que representa un elemento del menú en la aplicación de administración del blog. Cada elemento del menú puede tener un título, una URL, un icono, un orden de visualización y un estado de activación. Además, cada elemento del menú puede tener una lista de elementos hijos para representar submenús.
/// </summary>
public class MenuItem
{
        /// <summary>
        /// Identificador único del elemento del menú. Este campo es utilizado para diferenciar cada elemento del menú y establecer relaciones jerárquicas entre ellos (por ejemplo, para submenús). Es un campo clave en la estructura de datos del menú.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador del elemento del menú padre. Este campo es opcional y se utiliza para establecer la relación jerárquica entre los elementos del menú. Si el valor es nulo, significa que el elemento del menú es un elemento raíz (no tiene un padre). Si tiene un valor, ese valor corresponde al Id de otro elemento del menú que es su padre.
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Título del elemento del menú. Este campo es obligatorio y se utiliza para mostrar el nombre del elemento del menú en la interfaz de usuario. El título debe ser descriptivo para que los usuarios puedan entender fácilmente la función o el destino del elemento del menú.
        /// </summary>

        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// URL del elemento del menú. Este campo es obligatorio y se utiliza para establecer la dirección a la que redirige el elemento del menú cuando es seleccionado.
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Icono del elemento del menú. Este campo es opcional y se utiliza para mostrar un ícono representativo del elemento del menú en la interfaz de usuario.
        /// </summary>
        public string Icon { get; set; } = string.Empty;

        /// <summary>
        /// Orden de visualización del elemento del menú. Este campo es obligatorio y se utiliza para determinar el orden en que se muestran los elementos del menú en la interfaz de usuario.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Estado de activación del elemento del menú. Este campo es obligatorio y se utiliza para determinar si el elemento del menú está activo o no en la interfaz de usuario.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Lista de elementos hijos del elemento del menú. Este campo es opcional y se utiliza para representar submenús.
        /// </summary>
        public List<MenuItem> Children { get; set; } = new();
}
