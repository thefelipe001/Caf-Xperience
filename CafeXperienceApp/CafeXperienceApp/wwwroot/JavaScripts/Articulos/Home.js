$(document).ready(function () {

    // Mostrar Listado de Marcas
    var table = $('#tabla').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: "/Articulo/Data",
            type: 'POST',
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            beforeSend: function () {
                $('#loading-spinner').removeClass('d-none'); // Mostrar el spinner antes de cargar los datos
            },
            complete: function () {
                $('#loading-spinner').addClass('d-none'); // Ocultar el spinner cuando los datos estén listos
            }
        },
        "responsive": true,
        "columnDefs": [
            {
                "targets": -1,
                "data": null,
                "render": function (data, type, row, meta) {
                    return $("<div>").addClass("d-grid gap-2 d-md-flex justify-content-md-start")
                        .append(
                            $("<button>").addClass("btn btn-primary btn-editar btn-sm me-md-2")
                                .append($("<i>").addClass("fas fa-pen"))
                                .attr({ "data-informacion": JSON.stringify(row) })
                        )
                        .append(
                            $("<button>").addClass("btn btn-danger btn-eliminar btn-sm")
                                .append($("<i>").addClass("fas fa-trash"))
                                .attr({ "data-informacion": JSON.stringify(row) })
                        )[0].outerHTML;
                },
                "sortable": false
            },
            { "name": "IdArticulo", "data": "idArticulo", "targets": 0, "visible": true },
            { "name": "Descripcion", "data": "descripcion", "targets": 1 },
            { "name": "Costo", "data": "costo", "targets": 2 },
            { "name": "Existencia", "data": "existencia", "targets": 3 },
            {
                "name": "Imagen", "data": "rutaImagen", "targets": 4,
                "render": function (data) {
                    return `<img src="${data}" alt="estado" width="200px" />`;
                }
            },
            {
                "name": "Estado", "data": "estado", "targets": 5,
                "render": function (data) {
                    return data === "A" ? '<span class="badge bg-success">Activo</span>' : '<span class="badge bg-danger">No Activo</span>';
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

    // Abrir el Formulario (crear o editar)
    window.abrirModal = function (json) {
        $("#cboEstado").html(`
            <option value="">Seleccionar</option>
            <option value="1">Activo</option>
            <option value="0">No Activo</option>
        `);

        $("#IdMarca").val(json ? json.idMarca : 0);
        $("#txtdescripcion").val(json ? json.descripcion : "");
        $("#cboEstado").val(json ? (json.estado === "A" ? 1 : 0) : "");

        $('#FormModal').modal('show');
    }

    // Validación de formulario
    function validarFormulario() {
        if ($("#txtdescripcion").val().trim() === "") {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'El campo Descripción es obligatorio.',
                confirmButtonText: 'Aceptar'
            });
            return false;
        }

        if ($("#txtcosto").val().trim() === "") {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'El campo Descripción es obligatorio.',
                confirmButtonText: 'Aceptar'
            });
            return false;
        }
        if ($("#txtexistencia").val().trim() === "") {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'El campo Descripción es obligatorio.',
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

        if ($("#RutaImagen").get(0).files.length === 0) {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'Debe seleccionar una imagen para continuar.',
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
        formData.append('IdArticulo', $("#IdArticulo").val());
        formData.append('Descripcion', $("#txtdescripcion").val());
        formData.append('IdMarca', $("#idMarca").val());
        formData.append('IdProveedor', $("#idProveedor").val());
        formData.append('Costo', $("#txtcosto").val());
        formData.append('Existencia', $("#txtexistencia").val());
        formData.append('Estado', $("#cboEstado").val());
        formData.append('RutaImagen', $("#RutaImagen").val());

        // Agregar la imagen al formData
        let file = $("#RutaImagen")[0].files[0];
        formData.append('Imagen', file);

        try {
            let response = await $.ajax({
                url: "/Articulo/Guardar",
                type: "POST",
                data: formData,
                processData: false, // No procesar los datos
                contentType: false // No establecer el tipo de contenido (es necesario para enviar archivos)
            });

            if (response.resultado) {
                table.ajax.reload();
                $('#FormModal').modal('hide');
                Swal.fire('Guardado!', 'Cambios se guardaron exitosamente.', 'success');
            } else {
                Swal.fire('Error!', 'No se pudo guardar los cambios.', 'error');
            }

        } catch (error) {
            console.log(error);
            Swal.fire('Error!', 'Hubo un problema al enviar la solicitud.', 'error');
        }
    }

    // Consultar el Campus
    $.ajax({
        url: "/Articulo/DataMarca",
        type: "POST",
        dataType: "json",
        success: function (response) {
            // Limpiar el select antes de agregar nuevas opciones
            $("#idMarca").empty();

            // Iterar sobre la respuesta y agregar las nuevas opciones
            $.each(response.data, function (index, value) {
                $("<option>").attr({ "value": value.idMarca }).text(value.descripcion).appendTo("#idMarca");
            });
        },
        error: function (xhr, status, error) {
            console.error("Error: ", error);
        },
        beforeSend: function () {
            console.log("Cargando Marca...");
        }
    });

    // Consultar el Campus
    $.ajax({
        url: "/Articulo/DataProveedores",
        type: "POST",
        dataType: "json",
        success: function (response) {
            // Limpiar el select antes de agregar nuevas opciones
            $("#idProveedor").empty();

            // Iterar sobre la respuesta y agregar las nuevas opciones
            $.each(response.data, function (index, value) {
                $("<option>").attr({ "value": value.idProveedor }).text(value.nombreComercial).appendTo("#idProveedor");
            });
        },
        error: function (xhr, status, error) {
            console.error("Error: ", error);
        },
        beforeSend: function () {
            console.log("Cargando Proveedores...");
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
                        url: "/Articulo/Eliminar",
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
