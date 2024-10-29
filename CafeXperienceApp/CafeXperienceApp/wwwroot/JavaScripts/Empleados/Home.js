$(document).ready(function () {

    // Mostrar Listado de Tipo Usuarios
    var table = $('#tabla').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            url: "/Empleados/Data",
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
                "targets": -1, // Ultima columna para botones
                "data": null,
                "render": function (data, type, row, meta) {
                    return $("<div>").addClass("d-grid gap-2 d-md-flex justify-content-md-start") // Botones se acomodan en pantallas móviles
                        .append(
                            $("<button>").addClass("btn btn-primary btn-editar btn-sm me-md-2 d-block d-md-inline-block") // Botón "editar"
                                .append($("<i>").addClass("fas fa-pen"))
                                .attr({ "data-informacion": JSON.stringify(row) }) // Atributo data-informacion para editar
                        )
                        .append(
                            $("<button>").addClass("btn btn-danger btn-eliminar btn-sm d-block d-md-inline-block") // Botón "eliminar"
                                .append($("<i>").addClass("fas fa-trash"))
                                .attr({ "data-informacion": JSON.stringify(row) }) // Atributo data-informacion para eliminar
                        )[0].outerHTML;
                },
                "sortable": false // Deshabilita ordenación en esta columna
            },
            { "name": "IdEmpleado", "data": "idEmpleado", "targets": 0, "visible": true }, 
            { "name": "Nombre", "data": "nombre", "targets": 1 }, 
            {
                "name": "Cedula",
                "data": "cedula",
                "targets": 2,
                "render": function (data, type, row) {
                    if (!data) return ''; // Manejo de valores nulos o vacíos

                    // Aplicar formato a la cédula: 000-0000000-0
                    return data.replace(/(\d{3})(\d{7})(\d{1})/, '$1-$2-$3');
                }
            },
            { "name": "TandaLabor", "data": "tandaLabor", "targets": 3 }, 
            {
                "name": "PorcientoComision",
                "data": "porcientoComision",
                "targets": 4,
                "render": function (data, type, row) {
                    if (data == null) return ''; // Manejo de valores nulos o vacíos

                    // Formatear con 2 decimales y añadir símbolo de porcentaje
                    return parseFloat(data).toFixed(2) + ' %';
                }
            },
            {
                "name": "FechaIngreso",
                "data": "fechaIngreso",
                "targets": 5,
                "render": function (data, type, row) {
                    if (!data) return ''; // Manejo de valores nulos o vacíos

                    // Crear un objeto Date a partir de la fecha
                    const fecha = new Date(data);

                    // Formatear la fecha como dd/MM/yyyy
                    const dia = ('0' + fecha.getDate()).slice(-2);
                    const mes = ('0' + (fecha.getMonth() + 1)).slice(-2); // Los meses son base 0
                    const anio = fecha.getFullYear();

                    return `${dia}/${mes}/${anio}`;
                }
            }
            ,
            {
                "name": "Estado", "data": "estado", "targets": 6, // Columna para Estado
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
        // Crear opciones de estado como bloque de HTML
        const opciones = `
        <option value="">Seleccionar</option>
        <option value="1">Activo</option>
        <option value="0">No Activo</option>
    `;

        // Insertar las opciones en el combo de estado
        $("#cboEstado").html(opciones);

        // Restablecer los campos del formulario
        $("#IdEmpleado").val(""); // Restablece el ID del empleado
        $("#Nombre").val("");
        $("#Cedula").val("");
        $("#TandaLabor").val("");
        $("#PorcientoComision").val("");
        $("#fechaRegistro").val("");
        $("#cboEstado").val("");

        // Si se recibe un objeto JSON, llenar los campos con sus valores
        if (json) {
            $("#IdEmpleado").val(json.idEmpleado);
            $("#Nombre").val(json.nombre);
            $("#Cedula").val(json.cedula);
            $("#TandaLabor").val(json.tandaLabor);
            $("#PorcientoComision").val(json.porcientoComision);
            // Validar si la fechaRegistro es válida antes de intentar formatearla
            if (json.fechaRegistro) {
                const fecha = new Date(json.fechaRegistro);

                if (!isNaN(fecha)) {
                    // Si la fecha es válida, formatearla para el input datetime-local
                    const fechaFormato = fecha.toISOString().slice(0, 16); // yyyy-MM-ddTHH:mm
                    $("#fechaRegistro").val(fechaFormato);
                } else {
                    console.error("Fecha inválida:", json.fechaRegistro);
                    $("#fechaRegistro").val(''); // Opción: dejar el campo vacío si la fecha es inválida
                }
            } else {
                $("#fechaRegistro").val(''); // Si no hay fecha, dejar el campo vacío
            }

            // Asignar estado
            $("#cboEstado").val(json.estado ? 1 : 0);

            // Mostrar el modal
            $('#FormModal').modal('show');
            $("#cboEstado").val(json.estado ? 1 : 0); // Asignar estado
        }

        // Mostrar el modal
        $('#FormModal').modal('show');
    };



    // Función de validación de los campos del formulario
    function validarFormulario() {
        let isValid = true;

        // Validación del campo Nombre
        const nombre = $("#Nombre").val().trim();
        if (nombre === "") {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'El campo Nombre es obligatorio.',
                confirmButtonText: 'Aceptar'
            });
            return false; // Detener si no es válido
        }

        // Validación del campo Cedula
        const cedula = $("#Cedula").val().trim();
        const regexCedula = /^\d{3}\d{7}\d{1}$/; // Expresión regular para cédula

        if (!regexCedula.test(cedula)) {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'La cédula es requerida.',
                confirmButtonText: 'Aceptar'
            });
            return false;
        }

        // Validación del campo TandaLabor
        const tandaLabor = $("#TandaLabor").val().trim();
        if (tandaLabor === "") {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'El campo Horario de Trabajo es obligatorio.',
                confirmButtonText: 'Aceptar'
            });
            return false;
        }

        // Validación del campo PorcientoComision
        const porcientoComision = parseFloat($("#PorcientoComision").val());
        if (isNaN(porcientoComision) || porcientoComision < 0 || porcientoComision > 100) {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'El Porciento de Comisión debe ser un número entre 0 y 100.',
                confirmButtonText: 'Aceptar'
            });
            return false;
        }

        // Validación del campo Fecha Registro
        const fechaRegistro = $("#fechaRegistro").val().trim();
        if (fechaRegistro === "") {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'Debe seleccionar una fecha de registro.',
                confirmButtonText: 'Aceptar'
            });
            return false;
        }

        // Validación del campo Estado
        const estado = $("#cboEstado").val();
        if (!estado || estado.trim() === "") {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: 'Debe seleccionar un estado.',
                confirmButtonText: 'Aceptar'
            });
            return false;
        }

        return isValid; // Si todo es válido, devolver true
    }

    // Función para guardar el registro usando serialize
    window.Guardar = function () {
        // Validar los campos del formulario
        if (!validarFormulario()) {
            return; // Detener si la validación falla
        }

        var formData = $("#formNivel").serialize(); // Serializamos los datos del formulario

        $.ajax({
            url: "/Empleados/Guardar",
            type: "POST",
            data: formData, // Enviar los datos serializados
            beforeSend: function () {
                // Mostrar el spinner o deshabilitar botón de guardar
                $('#guardarBtn').prop('disabled', true);
                Swal.fire({
                    title: 'Guardando...',
                    html: 'Por favor espera un momento.',
                    allowOutsideClick: false,
                    didOpen: () => {
                        Swal.showLoading(); // Mostrar el spinner de carga
                    }
                });
            },
            success: function (data) {
                if (data.resultado) {
                    table.ajax.reload(); // Recargar DataTable
                    $('#FormModal').modal('hide'); // Cerrar el modal

                    Swal.fire({
                        icon: 'success',
                        title: 'Guardado!',
                        text: 'Cambios guardados exitosamente.',
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
                console.error(error); // Mostrar el error en la consola

                // Mostrar SweetAlert2 en caso de error en la solicitud
                Swal.fire({
                    icon: 'error',
                    title: 'Error!',
                    text: 'Hubo un problema al enviar la solicitud.',
                    confirmButtonText: 'Aceptar'
                });
            },
            complete: function () {
                // Habilitar el botón de guardar nuevamente
                $('#guardarBtn').prop('disabled', false);
                Swal.close(); // Cerrar el spinner de carga
            }
        });
    }

    // Abrir Formulario editar
    $(document).on('click', '.btn-editar', function (event) {
        var json = $(this).attr("data-informacion"); // Corregido a .attr() para obtener el valor
        var rowData = JSON.parse(json); // Convertimos el JSON a objeto
        $("#IdTipoUsuarios").val(rowData.idTipoUsuarios); // Asigna el ID al campo oculto

        abrirModal(rowData); // Llama a la función para abrir el modal con los datos
    });

    // Eliminar Registro

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
                        url: "/Empleados/Eliminar",
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
