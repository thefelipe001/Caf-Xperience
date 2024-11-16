$(document).ready(function () {

    // Mostrar Listado de Marcas
    var table = $('#tabla').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: "/FacturacionArticulo/Data",
            type: 'POST',
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            beforeSend: function () {
                $('#loading-spinner').removeClass('d-none'); // Mostrar el spinner antes de cargar los datos
            },
            complete: function () {
                $('#loading-spinner').addClass('d-none'); // Ocultar el spinner cuando los datos estén listos
            },
            error: function (xhr, status, error) {
                console.error('Error al cargar los datos:', error);
                Swal.fire('Error', 'No se pudieron cargar los datos.', 'error');
            }
        },
        "responsive": true,
        "columnDefs": [
            {
                "targets": -1,
                "data": null,
                "render": function (data, type, row) {
                    return $("<div>").addClass("d-grid gap-2 d-md-flex justify-content-md-start")
                        .append(
                            $("<button>").addClass("btn btn-primary btn-editar btn-sm me-md-2")
                                .attr({
                                    "data-informacion": JSON.stringify(row),
                                    "aria-label": "Editar"
                                })
                                .append($("<i>").addClass("fas fa-pen"))
                        )
                        .append(
                            $("<button>").addClass("btn btn-danger btn-eliminar btn-sm")
                                .attr({
                                    "data-informacion": JSON.stringify(row),
                                    "aria-label": "Eliminar"
                                })
                                .append($("<i>").addClass("fas fa-trash"))
                        )[0].outerHTML;
                },
                "sortable": false
            },
            { "name": "NoFactura", "data": "noFactura", "targets": 0, "visible": true },
            { "name": "IdArticulo", "data": "idArticulo", "targets": 1 },
            { "name": "UnidadesVendidas", "data": "unidadesVendidas", "targets": 2 },
            { "name": "Monto", "data": "montoArticulo", "targets": 3 },
            { "name": "Comentario", "data": "comentario", "targets": 4 },
            {
                "name": "Estado", "data": "estado", "targets": 5,
                "render": function (data) {
                    return data === "A"
                        ? '<span class="badge bg-success">Activo</span>'
                        : '<span class="badge bg-danger">No Activo</span>';
                }
            }
        ],
        "order": [[0, "desc"]],
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
        "pagingType": "full_numbers"
    });

    // Asignar eventos a los botones después de la inicialización de la tabla
    $('#tabla tbody').on('click', '.btn-editar', function () {
        let informacion = JSON.parse($(this).attr('data-informacion'));
        console.log('Editar:', informacion);
        // Lógica para editar
    });

    $('#tabla tbody').on('click', '.btn-eliminar', function () {
        let informacion = JSON.parse($(this).attr('data-informacion'));
        Swal.fire({
            title: '¿Estás seguro?',
            text: "Esta acción no se puede revertir.",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí, eliminar'
        }).then((result) => {
            if (result.isConfirmed) {
                console.log('Eliminar:', informacion);
                Swal.fire('Eliminado!', 'El registro ha sido eliminado.', 'success');
                // Lógica para eliminar
            }
        });
    });

    // Abrir el Formulario (crear o editar)
    window.abrirModal = function (json) {
        $("#cboEstado").html(`
            <option value="">Seleccionar</option>
            <option value="1">Activo</option>
            <option value="0">No Activo</option>
        `);



        $("#txtUnidadesVendidas").val(json ? json.unidadesVendidas : 0);
        $("#NoFactura").val(json ? json.noFactura : 0);
        $("#txtMontoArticulo").val(json ? json.montoArticulo : 0);
        $("#txtComentario").val(json ? json.comentario : "");

        $("#idEmpleado").val(json ? json.idEmpleado : "");
        $("#idArticulo").val(json ? json.idArticulo : "");
        $("#idUsuario").val(json ? json.idUsuario : "");
        $("#idCampus").val(json ? json.idCampus : "");

        $("#cboEstado").val(json ? (json.estado === "A" ? 1 : 0) : "");

        $('#FormModal').modal('show');
    }

    // Validación de formulario
    function validarFormulario() {

        if ($("#txtMontoArticulo").val().trim() === "") {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'El campo monto es obligatorio.',
                confirmButtonText: 'Aceptar'
            });
            return false;
        }
        if ($("#txtUnidadesVendidas").val().trim() === "") {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'El campo unidades vendidas es obligatorio.',
                confirmButtonText: 'Aceptar'
            });
            return false;
        }

        if ($("#cboEstado").val().trim() === "") {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'Debe seleccionar un estado.',
                confirmButtonText: 'Aceptar'
            });
            return false;
        }

       

        return true;
    }
    // Guardar datos del formulario (incluyendo la imagen)
    window.Guardar = async function () {
        if (!validarFormulario()) return;

        let formData = new FormData();
        let fields = [
            { key: 'NoFactura', value: $("#NoFactura").val() },
            { key: 'IdEmpleado', value: $("#idEmpleado").val() },
            { key: 'IdArticulo', value: $("#idArticulo").val() },
            { key: 'IdUsuario', value: $("#idUsuario").val() },
            { key: 'IdCampus', value: $("#idCampus").val() },
            { key: 'UnidadesVendidas', value: $("#txtUnidadesVendidas").val() },
            { key: 'MontoArticulo', value: $("#txtMontoArticulo").val() },
            { key: 'Comentario', value: $("#txtComentario").val() },
            { key: 'Estado', value: $("#cboEstado").val() },
        ];

        // Añadir todos los campos al FormData
        fields.forEach(field => formData.append(field.key, field.value));

       

        try {
            // Deshabilitar el botón para evitar múltiples envíos
            let btnGuardar = $('#btnGuardar');
            btnGuardar.prop('disabled', true);

            let response = await $.ajax({
                url: "/FacturacionArticulo/Guardar",
                type: "POST",
                data: formData,
                processData: false, // No procesar los datos
                contentType: false // No establecer el tipo de contenido
            });

            if (response.resultado) {
                table.ajax.reload(); // Recargar tabla con los cambios
                $('#FormModal').modal('hide'); // Cerrar modal
                Swal.fire('Guardado!', 'Cambios se guardaron exitosamente.', 'success');
            } else {
                Swal.fire('Error!', 'No se pudo guardar los cambios.', 'error');
            }

        } catch (error) {
            console.error('Error en la solicitud:', error);
            Swal.fire('Error!', 'Hubo un problema al enviar la solicitud.', 'error');
        } finally {
            // Rehabilitar el botón al finalizar el proceso
            $('#btnGuardar').prop('disabled', false);
        }
    };


    // Consultar el articulo
    $.ajax({
        url: "/FacturacionArticulo/DataArticulo",
        type: "POST",
        dataType: "json",
        success: function (response) {
            // Limpiar el select antes de agregar nuevas opciones
            $("#idArticulo").empty();

            // Iterar sobre la respuesta y agregar las nuevas opciones
            $.each(response.data, function (index, value) {
                $("<option>").attr({ "value": value.idArticulo }).text(value.descripcion).appendTo("#idArticulo");
            });
        },
        error: function (xhr, status, error) {
            console.error("Error: ", error);
        },
        beforeSend: function () {
            console.log("Cargando articulos...");
        }
    });

    // Consultar el empleado
    $.ajax({
        url: "/FacturacionArticulo/DataEmpleado",
        type: "POST",
        dataType: "json",
        success: function (response) {
            // Limpiar el select antes de agregar nuevas opciones
            $("#idEmpleado").empty();

            // Iterar sobre la respuesta y agregar las nuevas opciones
            $.each(response.data, function (index, value) {
                $("<option>").attr({ "value": value.idEmpleado }).text(value.nombre).appendTo("#idEmpleado");
            });
        },
        error: function (xhr, status, error) {
            console.error("Error: ", error);
        },
        beforeSend: function () {
            console.log("Cargando empleados...");
        }
    });

    // Consultar el usuario
    $.ajax({
        url: "/FacturacionArticulo/DataUsuario",
        type: "POST",
        dataType: "json",
        success: function (response) {
            // Limpiar el select antes de agregar nuevas opciones
            $("#idUsuario").empty();

            // Iterar sobre la respuesta y agregar las nuevas opciones
            $.each(response.data, function (index, value) {
                $("<option>").attr({ "value": value.idUsuario }).text(value.nombre).appendTo("#idUsuario");
            });
        },
        error: function (xhr, status, error) {
            console.error("Error: ", error);
        },
        beforeSend: function () {
            console.log("Cargando usuarios...");
        }
    });

    $.ajax({
        url: "/FacturacionArticulo/DataCampus",
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
            console.log("Cargando CAMPUS...");
        }
    });


    // Abrir Formulario para editar
    $(document).on('click', '.btn-editar', function () {
        let rowData = JSON.parse($(this).attr("data-informacion"));
        abrirModal(rowData);
    });

    // Eliminar registro
    $(document).on('click', '.btn-eliminar', function () {
        let dataObj = JSON.parse($(this).attr("data-informacion"));

        Swal.fire({
            title: '¿Estás seguro?',
            text: "¡No podrás revertir esto!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminarlo!',
            cancelButtonText: 'Cancelar',
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
        }).then(async (result) => {
            if (result.isConfirmed) {
                try {
                    let response = await $.ajax({
                        url: "/FacturacionArticulo/Eliminar",
                        type: "POST",
                        data: JSON.stringify(dataObj),
                        contentType: "application/json; charset=utf-8"
                    });

                    if (response.resultado) {
                        table.ajax.reload();
                        Swal.fire('Eliminado!', 'El registro ha sido eliminado.', 'success');
                    } else {
                        Swal.fire('Error!', response.mensaje, 'error');
                    }
                } catch (error) {
                    console.log(error);
                    Swal.fire('Error!', 'Hubo un problema al enviar la solicitud.', 'error');
                }
            }
        });
    });


});
