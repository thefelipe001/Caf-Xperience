$(document).ready(function () {
    // Mostrar Listado de Reporte de Renta
    table = $('#tabla').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: "/Facturas/Data",
            type: 'POST',
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            data: function (d) {
                // Agrega parámetros personalizados si es necesario
                d.Usuario = $('#searchUsuario').val();
                d.Campus = $('#searchCampus').val();
                d.Fecha = $('#searchFecha').val();
                d.Proveedor = $('#searchProveedor').val();
                d.Monto = $('#searchMonto').val();
            },
            beforeSend: function () {
                // Mostrar el spinner antes de cargar los datos
                $('#loading-spinner').removeClass('d-none');
            },
            complete: function () {
                // Ocultar el spinner cuando los datos estén listos
                $('#loading-spinner').addClass('d-none');
            }
        },
        "responsive": true, // Hace la tabla responsive
        "columnDefs": [
            { "name": "Numero Factura", "data": "numFactura", "targets": 0, "visible": true }, 
            { "name": "DescripcionArticulo", "data": "descripcionArticulo", "targets": 1, "visible": true }, 
            {
                "name": "FechaVenta",
                "data": "fechaVenta",
                "targets": 2,
                "render": function (data) {
                    const date = new Date(data);
                    return date.toLocaleDateString('es-ES');
                }
            },
           
            {
                "name": "Monto",
                "data": "monto",
                "targets": 3,
                "render": function (data) {
                    return parseFloat(data).toLocaleString('es-ES', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                }
            },
            { "name": "Cantidad", "data": "cantidad", "targets": 4 }, 

            { "name": "Comentario", "data": "comentario", "targets": 5 }, 
            { "name": "DescripcionCampus", "data": "descripcionCampus", "targets": 6 }, 
            { "name": "RNC", "data": "rnc", "targets": 7 }, // RNC del proveedor
            { "name": "NombreComercial", "data": "nombreComercial", "targets": 8 }, 
            
        ],
        "order": [[0, "desc"]], // Ordenar por la primera columna (IdTipoUsuarios)
        "language": {
            "processing": "Procesando...",
            "lengthMenu": "Mostrar _MENU_ registros",
            "zeroRecords": "No se encontraron resultados",
            "emptyTable": "Ningún dato disponible en esta tabla",
            "info": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
            "infoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
            "infoFiltered": "(filtrado de un total de _MAX_ registros)",
            "search": "Buscar:",
            "paginate": {
                "first": "Primero",
                "last": "Último",
                "next": "Siguiente",
                "previous": "Anterior"
            },
            "loadingRecords": "Cargando...",
            "aria": {
                "sortAscending": ": Activar para ordenar la columna de manera ascendente",
                "sortDescending": ": Activar para ordenar la columna de manera descendente"
            }
        },
        "dom": '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>' +
            '<"row"<"col-sm-12"tr>>' +
            '<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
        "pagingType": "full_numbers" // Estilo de paginación
    });


    function cargarCampus() {
        $.ajax({
            url: window.location.origin + "/Facturas/DataCampus", // Asegúrate de que esta URL sea correcta
            type: "POST",
            dataType: "json",
            success: function (response) {
                // Limpiar el select antes de agregar nuevas opciones, pero mantener la opción inicial
                $("#searchCampus").find("option:not([value=''])").remove();

                // Crear un Set para almacenar los valores únicos de `idCampus`
                let opcionesUnicas = new Set();

                // Iterar sobre la respuesta y agregar las nuevas opciones si no están duplicadas
                $.each(response.data, function (index, value) {
                    // Solo agregar si el idCampus no está en el Set
                    if (!opcionesUnicas.has(value.idCampus)) {
                        opcionesUnicas.add(value.idCampus); // Añadir el idCampus al Set para evitar duplicados
                        $("<option>")
                            .attr({ "value": value.descripcion })
                            .text(value.descripcion)
                            .appendTo("#searchCampus");
                    }
                });
            },
            error: function (xhr, status, error) {
                console.error("Error: ", error);
            },
            beforeSend: function () {
                console.log("Cargando Campus...");
            }
        });
    }

    cargarCampus();

    function cargarUsuario() {
        $.ajax({
            url: window.location.origin + "/Facturas/DataUsuario", // Asegúrate de que esta URL sea correcta
            type: "POST",
            dataType: "json",
            success: function (response) {
                $("#searchUsuario").find("option:not([value=''])").remove();
                let opcionesUnicas = new Set();

                $.each(response.data, function (index, value) {
                    if (!opcionesUnicas.has(value.idUsuario)) {
                        opcionesUnicas.add(value.idUsuario);
                        console.log("value: ", value);
                        $("<option>")
                            .attr({ "value": value.nombre })
                            .text(value.nombre)
                            .appendTo("#searchUsuario");
                    }
                });
            },
            error: function (xhr, status, error) {
                console.error("Error: ", error);
            },
            beforeSend: function () {
                console.log("Cargando Usuario...");
            }
        });
    }

    cargarUsuario();

    function cargarProveedor() {
        $.ajax({
            url: window.location.origin + "/Facturas/DataProveedor", // Asegúrate de que esta URL sea correcta
            type: "POST",
            dataType: "json",
            success: function (response) {
                // Limpiar el select antes de agregar nuevas opciones, pero mantener la opción inicial
                $("#searchProveedor").find("option:not([value=''])").remove();

                // Crear un Set para almacenar los valores únicos de `idProveedor`
                let opcionesUnicas = new Set();

                // Iterar sobre la respuesta y agregar las nuevas opciones si no están duplicadas
                $.each(response.data, function (index, value) {
                    // Solo agregar si el idProveedor no está en el Set
                    if (!opcionesUnicas.has(value.idProveedor)) {
                        opcionesUnicas.add(value.idProveedor); // Añadir el idProveedor al Set para evitar duplicados
                        $("<option>")
                            .attr({ "value": value.nombreComercial })
                            .text(value.nombreComercial)
                            .appendTo("#searchProveedor");
                    }
                });
            },
            error: function (xhr, status, error) {
                console.error("Error: ", error);
            },
            beforeSend: function () {
                console.log("Cargando Proveedor...");
            }
        });
    }

    cargarProveedor();
});


