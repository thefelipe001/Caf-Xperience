$(document).ready(function () {

    // Mostrar Listado de Campus
    var table = $('#tabla').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: "/TipoUsuarios/Data",
            type: 'POST',
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            data: function (d) {
                // Agrega par�metros personalizados si es necesario
            },
            beforeSend: function () {
                // Mostrar el spinner antes de cargar los datos
                $('#loading-spinner').removeClass('d-none');
            },
            complete: function () {
                // Ocultar el spinner cuando los datos est�n listos
                $('#loading-spinner').addClass('d-none');
            }
        },
        "responsive": true, // Hace la tabla responsive
        "columnDefs": [
            {
                "targets": -1, // Ultima columna para botones
                "data": null,
                "render": function (data, type, row, meta) {
                    return $("<div>").addClass("d-grid gap-2 d-md-flex justify-content-md-start") // Botones se acomodan en pantallas m�viles
                        .append(
                            $("<button>").addClass("btn btn-primary btn-editar btn-sm me-md-2 d-block d-md-inline-block") // Bot�n "editar"
                                .append($("<i>").addClass("fas fa-pen"))
                                .attr({ "data-informacion": JSON.stringify(row) }) // Atributo data-informacion para editar
                        )
                        .append(
                            $("<button>").addClass("btn btn-danger btn-eliminar btn-sm d-block d-md-inline-block") // Bot�n "eliminar"
                                .append($("<i>").addClass("fas fa-trash"))
                                .attr({ "data-informacion": JSON.stringify(row) }) // Atributo data-informacion para eliminar
                        )[0].outerHTML;
                },
                "sortable": false // Deshabilita ordenaci�n en esta columna
            },
            { "name": "IdTipoUsuarios", "data": "idTipoUsuarios", "targets": 0, "visible": true }, // Columna para IdTipoUsuarios
            { "name": "Descripcion", "data": "descripcion", "targets": 1 }, // Columna para Descripci�n
            {
                "name": "Estado", "data": "estado", "targets": 2, // Columna para Estado
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
            "emptyTable": "Ning�n dato disponible en esta tabla",
            "info": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
            "infoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
            "infoFiltered": "(filtrado de un total de _MAX_ registros)",
            "search": "Buscar:",
            "paginate": {
                "first": "Primero",
                "last": "�ltimo",
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
        "pagingType": "full_numbers" // Estilo de paginaci�n
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
        $("#IdTipoUsuarios").val(0);
        $("#txtdescripcion").val("");
        $("#cboEstado").val("");

        if (json != null) {
            $("#IdTipoUsuarios").val(json.idTipoUsuarios); // Aseg�rate de usar los nombres correctos
            $("#txtdescripcion").val(json.descripcion);
            $("#cboEstado").val(json.estado == "A" ? 1 : 0);
        }

        $('#FormModal').modal('show');
    }


    // Funci�n de validaci�n de los campos del formulario
    function validarFormulario() {
        var isValid = true;
        var descripcion = $("#txtdescripcion").val().trim();
        // Validar que la descripci�n no est� vac�a
        if (descripcion === "") {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'El campo Descripci�n es obligatorio.',
                confirmButtonText: 'Aceptar'
            });
            return false;
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


        return isValid; // Si todo es v�lido, devolver true
    }

    // Guardar Registro usando serialize
    window.Guardar = function () {
        // Primero validamos los campos del formulario
        if (!validarFormulario()) {
            return; // Si la validaci�n falla, no se ejecuta el c�digo de guardado
        }

        var formData = $("#formNivel").serialize(); // Serializamos los datos del formulario

        jQuery.ajax({
            url: "/TipoUsuarios/Guardar",
            type: "POST",
            data: formData, // Enviamos los datos serializados
            success: function (data) {
                if (data.resultado) {
                    table.ajax.reload(); // Recarga la tabla DataTable
                    $('#FormModal').modal('hide'); // Cierra el modal
                    // Mostrar SweetAlert2 de �xito
                    Swal.fire({
                        icon: 'success',
                        title: 'Guardado!',
                        text: 'Cambios se guardaron exitosamente.',
                        confirmButtonText: 'Aceptar'
                    });
                } else {
                    // Mostrar SweetAlert2 de error
                    Swal.fire({
                        icon: 'error',
                        title: 'Error!',
                        text: 'No se pudo guardar los cambios.',
                        confirmButtonText: 'Aceptar'
                    });
                }
            },
            error: function (error) {
                console.log(error);
                // Mostrar SweetAlert2 en caso de error en la solicitud
                Swal.fire({
                    icon: 'error',
                    title: 'Error!',
                    text: 'Hubo un problema al enviar la solicitud.',
                    confirmButtonText: 'Aceptar'
                });
            },
            beforeSend: function () {
                // Mostrar el spinner si es necesario
            }
        });
    }


    // Abrir Formulario editar
    $(document).on('click', '.btn-editar', function (event) {
        var json = $(this).attr("data-informacion"); // Corregido a .attr() para obtener el valor
        var rowData = JSON.parse(json); // Convertimos el JSON a objeto
        $("#IdTipoUsuarios").val(rowData.idTipoUsuarios); // Asigna el ID al campo oculto

        abrirModal(rowData); // Llama a la funci�n para abrir el modal con los datos
    });

    // Eliminar Registro

    $(document).on('click', '.btn-eliminar', function (event) {
        var json = $(this).attr("data-informacion"); // Obtiene la informaci�n como string JSON
        var dataObj = JSON.parse(json); // Convierte el string JSON en un objeto JavaScript

        Swal.fire({
            title: '�Est�s seguro?',
            text: "�No podr�s revertir esto!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'S�, eliminarlo!'
        }).then((result) => {
            if (result.isConfirmed) {
                jQuery.ajax({
                    url: "/TipoUsuarios/Eliminar",
                    type: "POST",
                    data: JSON.stringify(dataObj), // Enviamos el modelo completo
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        if (data.resultado) {
                            table.ajax.reload(); // Recarga la tabla DataTable
                            Swal.fire(
                                'Eliminado!',
                                'El usuario ha sido eliminado.',
                                'success'
                            );
                        } else {
                            Swal.fire({
                                icon: 'error',
                                title: 'Error!',
                                text: data.mensaje,
                                confirmButtonText: 'Aceptar'
                            });
                        }
                    },
                    error: function (error) {
                        console.log(error);
                        Swal.fire({
                            icon: 'error',
                            title: 'Error!',
                            text: 'Hubo un problema al enviar la solicitud.',
                            confirmButtonText: 'Aceptar'
                        });
                    }
                });
            }
        });
    });

});
