$(document).ready(function () {

    // Mostrar Listado de Tipo Usuarios
    var table = $('#tabla').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: "/Usuarios/Data",
            type: 'POST',
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            data: function (d) {
                // Agrega parámetros personalizados si es necesario
                d.nombre = $('#searchNombre').val();
                d.cedula = $('#searchCedula').val();
                d.fechaRegistro = $('#searchFechaRegistro').val();
                d.estado = $('#searchEstado').val();
                d.limiteCredito = $('#searchLimiteCredito').val();
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
            { "name": "ID", "data": "idUsuario", "targets": 0, "visible": true }, // Columna para IdUsuarios
            { "name": "Nombre", "data": "nombre", "targets": 1 }, // Columna para Descripción
            { "name": "Cedula", "data": "cedula", "targets": 2 }, // Columna para Cedula
            {
                "name": "fechaRegistro",
                "data": "fechaRegistro",
                "targets": 4,
                "render": function (data) {
                    const date = new Date(data);
                    return date.toLocaleDateString('es-ES');
                }
            }, // Columna para Fecha
            {
                "name": "Limite Credito",
                "data": "limiteCredito",
                "targets": 3,
                "render": function (data) {
                    return parseFloat(data).toLocaleString('es-ES', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                }
            }, // Columna para Limite Credito
            { "name": "Correo", "data": "correo", "targets": 5 }, // Columna para correo
            { "name": "Rol", "data": "tipoUsuario", "targets": 6 }, // Columna para tipoUsuario
            {
                "name": "Estado",
                "data": "estado",
                "targets": 7, // Columna para Estado
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

        $("#IdUsuario").val(0);
        $("#Nombre").val("");
        $("#Cedula").val("");
        $("#fechaRegistro").val("");
        $("#limiteCredito").val(0);
        $("#Correo").val("");
        $("#Contraseña").val("");
        $("#cboEstado").val("");

        if (json != null) {
            $("#IdUsuario").val(json.idUsuario);
            $("#Nombre").val(json.nombre);
            $("#Cedula").val(json.cedula);
            $("#fechaRegistro").val(json.fechaRegistro);
            $("#limiteCredito").val(json.limiteCredito);
            $("#Correo").val(json.correo);
            $("#Contraseña").val(json.contraseña);
            $("#cboEstado").val(json.estado == "A" ? 1 : 0);
        }

        $('#FormModal').modal('show');
    }

    // Consultar el Tipo de usuarios
    $.ajax({
        url: "/Usuarios/DataTipoUsuarios",
        type: "POST",
        dataType: "json",
        success: function (response) {
            $.each(response.data, function (index, value) {
                $("<option>").attr({ "value": value.idTipoUsuarios }).text(value.descripcion).appendTo("#tipoUsuario");
            });
        },
        error: function (xhr, status, error) {
            console.error("Error: ", error);
        },
        beforeSend: function () {
            console.log("Cargando tipos de usuarios...");
        }
    });

    // Guardar Usuarios
    window.Guardar = function () {
        if (!validarFormulario()) {
            return;
        }

        var formData = $("#formNivel").serialize();

        jQuery.ajax({
            url: "/Usuarios/Guardar",
            type: "POST",
            data: formData, // Enviamos los datos serializados
            success: function (data) {
                if (data.resultado) {
                    if (data.resultado) {
                        table.ajax.reload();
                        $('#FormModal').modal('hide');
                        Swal.fire('Guardado!', 'Cambios se guardaron exitosamente.', 'success');
                        $('#FormModal').on('hidden.bs.modal', function () {
                            $(this).find('form')[0].reset(); // Restablece el formulario
                        });
                    } else {
                        Swal.fire('Error!', 'No se pudo guardar los cambios.', 'error');
                    }

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

        if ($("#Nombre").val().trim() === "") {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'El campo Nombre es obligatorio.',
                confirmButtonText: 'Aceptar'
            });
            isValid = false;
            return isValid;
        }

        if ($("#Cedula").val().trim() === "") {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'El campo Cédula es obligatorio.',
                confirmButtonText: 'Aceptar'
            });
            isValid = false;
            return isValid;
        }

        if ($("#fechaRegistro").val().trim() === "") {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'El campo Fecha de Registro es obligatorio.',
                confirmButtonText: 'Aceptar'
            });
            isValid = false;
            return isValid;
        }

        if ($("#limiteCredito").val() <= 0) {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'El campo Límite de Crédito debe ser mayor a 0.',
                confirmButtonText: 'Aceptar'
            });
            isValid = false;
            return isValid;
        }

        if ($("#Correo").val().trim() === "") {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'El campo Correo es obligatorio.',
                confirmButtonText: 'Aceptar'
            });
            isValid = false;
            return isValid;
        } else if (!validateEmail($("#Correo").val())) {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'El correo electrónico no es válido.',
                confirmButtonText: 'Aceptar'
            });
            isValid = false;
            return isValid;
        }

        if ($("#Contraseña").length && $("#Contraseña").val().trim() === "") {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'El campo Contraseña es obligatorio.',
                confirmButtonText: 'Aceptar'
            });
            isValid = false;
            return isValid;
        }

        if ($("#tipoUsuario").length && $("#tipoUsuario").val().trim() === "") {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'Debe seleccionar un tipo de usuario.',
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
    function eliminarUsuario(idUsuario) {
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
                    url: `/Usuarios/Eliminar?IdUsuario=${idUsuario}`,
                    type: "POST",
                    success: function (data) {
                        if (data.resultado) {
                            table.ajax.reload();
                            Swal.fire('Eliminado!', 'El usuario ha sido eliminado.', 'success');
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
        eliminarUsuario(dataObj.idUsuario); // Usa la función creada para eliminar
    });

   
});


