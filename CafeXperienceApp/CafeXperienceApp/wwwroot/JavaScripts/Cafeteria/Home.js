$(document).ready(function () {

    // Mostrar Listado de Tipo Usuarios
    var table = $('#tabla').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: "/Cafeteria/Data",
            type: 'POST',
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            data: function (d) {
                // Agrega parámetros personalizados si es necesario
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
            {
                "targets": -1, // Última columna para botones
                "data": null,
                "render": function (data, type, row, meta) {
                    return `
                    <div class="d-grid gap-2 d-md-flex justify-content-md-start">
                        <button class="btn btn-primary btn-editar btn-sm me-md-2 d-block d-md-inline-block" data-informacion='${JSON.stringify(row)}'>
                            <i class="fas fa-pen"></i>
                        </button>
                        <button class="btn btn-danger btn-eliminar btn-sm d-block d-md-inline-block" data-informacion='${JSON.stringify(row)}'>
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>`;
                },
                "sortable": false // Deshabilita ordenación en esta columna
            },
            { "name": "ID", "data": "idCafeteria", "targets": 0, "visible": true }, // Columna para IdUsuarios
            { "name": "Descripcion", "data": "descripcion", "targets": 1 }, // Columna para Descripción
            { "name": "Campus", "data": "campus", "targets": 2 }, // Columna para Descripción
            { "name": "Encargado", "data": "encargado", "targets": 3 }, // Columna para Descripción
            {
                "name": "Estado", "data": "estado", "targets": 4, // Columna para Estado
                "render": function (data) {
                    return data === "A" ? '<span class="badge bg-success">Activo</span>' : '<span class="badge bg-danger">No Activo</span>';
                }
            }
          
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

    // Abrir el Formulario
    window.abrirModal = function (json) {
        // Crear todas las opciones como un bloque de HTML
        var opciones = `
        <option value="">Seleccionar</option>
        <option value="1">Activo</option>
        <option value="0">No Activo</option>
    `;
        // Insertar todas las opciones de una vez
        $("#cboEstado").html(opciones);


        if (json != null) {
            $("#Nombre").val(json.nombre);
  
            $("#cboEstado").val(json.estado == "A" ? 1 : 0);
        }

        $('#FormModal').modal('show');
    }

    // Consultar el Campus
    $.ajax({
        url: "/Cafeteria/DataCampus",
        type: "POST",
        dataType: "json",
        success: function (response) {
            // Limpiar el select antes de agregar nuevas opciones
            $("#idCampus").empty();

            // Iterar sobre la respuesta y agregar las nuevas opciones
            $.each(response.data, function (index, value) {
                $("<option>").attr({ "value": value.idCampus }).text(value.descripcion).appendTo("#idCampus");
            });
        },
        error: function (xhr, status, error) {
            console.error("Error: ", error);
        },
        beforeSend: function () {
            console.log("Cargando Campus...");
        }
    });


  

    $.ajax({
        url: "/Cafeteria/DataEncargado",  // Ajusta la URL a tu endpoint
        type: "POST",
        dataType: "json",
        success: function (response) {
            // Verifica la estructura de la respuesta en la consola
            console.log(response);

            // Limpiar el select antes de agregar nuevas opciones
            $("#idEncargado").empty();

            // Verifica si la respuesta contiene datos
            if (response.data && response.data.length > 0) {
                // Iterar sobre la respuesta y agregar las nuevas opciones
                $.each(response.data, function (index, value) {
                    $("<option>").attr({ "value": value.idEmpleado }).text(value.nombre).appendTo("#idEncargado");
                });
            } else {
                console.warn("No se encontraron datos para llenar el select.");
            }
        },
        error: function (xhr, status, error) {
            console.error("Error: ", error);
        },
        beforeSend: function () {
            console.log("Cargando Encargados...");
        }
    });




    // Guardar Usuarios
    window.Guardar = function () {
        if (!validarFormulario()) {
            return;
        }

        var formData = $("#formNivel").serialize();

        jQuery.ajax({
            url: "/Cafeteria/Guardar",
            type: "POST",
            data: formData, // Enviamos los datos serializados
            success: function (data) {
                if (data.resultado) {
                    table.ajax.reload();
                    $('#FormModal').modal('hide');
                    Swal.fire({
                        icon: 'success',
                        title: 'Guardado!',
                        text: 'Cambios se guardaron exitosamente.',
                        confirmButtonText: 'Aceptar'
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error!',
                        text: 'No se pudo guardar los cambios.',
                        confirmButtonText: 'Aceptar'
                    });
                }
            },
            error: function (error) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error!',
                    text: 'Hubo un problema al enviar la solicitud.',
                    confirmButtonText: 'Aceptar'
                });
            }
        });
    };

    // Función de validación de los campos del formulario
    function validarFormulario() {
        var isValid = true;
        var idEncargado = $("#idEncargado").val().trim();

        if ($("#descripcion").val().trim() === "") {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'El campo Nombre es obligatorio.',
                confirmButtonText: 'Aceptar'
            });
            isValid = false;
            return isValid;
        }

        
        if ($("#cboEstado").length && $("#cboEstado").val().trim() === "") {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'Debe seleccionar un estado.',
                confirmButtonText: 'Aceptar'
            });
            isValid = false;
            return isValid;
        }

        return isValid; // Si todo es válido, devolver true
    }

    // Función para validar el formato del correo electrónico
    function validateEmail(email) {
        var re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return re.test(email);
    }

    // Abrir Formulario editar
    $(document).on('click', '.btn-editar', function (event) {
        var json = $(this).attr("data-informacion");
        var rowData = JSON.parse(json);
        abrirModal(rowData); // Llama a la función para abrir el modal con los datos
    });

    // Función para eliminar usuario
    function eliminarCafeteria(IdCafeteria) {
        Swal.fire({
            title: '¿Estás seguro?',
            text: "¡No podrás revertir esto!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí, eliminarlo!'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `/Cafeteria/Eliminar?IdCafeteria=${IdCafeteria}`,
                    type: "POST",
                    success: function (data) {
                        if (data.resultado) {
                            table.ajax.reload();
                            Swal.fire('Eliminado!', 'El Cafeteria ha sido eliminado.', 'success');
                        } else {
                            Swal.fire('Error!', data.mensaje, 'error');
                        }
                    },
                    error: function (error) {
                        Swal.fire('Error!', 'Hubo un problema al enviar la solicitud.', 'error');
                    }
                });
            }
        });
    }

    // Eliminar Registro
    $(document).on('click', '.btn-eliminar', function (event) {
        var json = $(this).attr("data-informacion");
        var dataObj = JSON.parse(json);
        eliminarCafeteria(dataObj.idCafeteria); // Usa la función creada para eliminar
    });
});
