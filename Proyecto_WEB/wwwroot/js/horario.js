var fechaIni = document.getElementById('fechaInicio');
var fechaF = document.getElementById('fechaFin');

const fechaActual = new Date().toISOString().split('T')[0];
fechaIni.min = fechaActual;
fechaF.min = fechaActual;

fechaIni.addEventListener('input', function () {
    fechaF.min = fechaIni.value;
});

function seleccionarTodos() {
    var checkboxes = document.querySelectorAll('input[type="checkbox"]');
    checkboxes.forEach(function (checkbox) {
        checkbox.checked = true;
    });
}

var fechasHorasProgramadas = [];
var horarios = [];
var recurso = null;

const selected = document.querySelector("#encargado");
selected.addEventListener("change", (event) => {
    if (recurso === null) {
        recurso = event.target.value;
    } else {
        if (confirm("¿Seguro que desea cambiar el recurso?\nSe borraran los datos ya asignados.")) {
            recurso = event.target.value;
            const tabla = document.getElementById('agenda');
            tabla.getElementsByTagName('tbody')[0].innerHTML = '';
            fechasHorasProgramadas = [];
            horarios = [];
        } else {
            selected.value = recurso;
        }
    }
});

function generarPlanificacion() {
    if (recurso === null) {
        alert("Seleccione un recurso.");
    } else {
        var fechaInicio = document.getElementById('fechaInicio').value;
        var horaInicio = document.getElementById('hora_inicio').value + ':00';
        var fechaFin = document.getElementById('fechaFin').value;
        var horaFin = document.getElementById('hora_fin').value + ':00';
        var diasSeleccionados = obtenerDiasSeleccionados();

        var intervalo = parseInt(document.getElementById('intervalo').value);

        var fecha = new Date(fechaInicio)
        fecha.setDate(fecha.getDate() + 1);
        var fecha_fin = new Date(fechaFin);
        fecha_fin.setDate(fecha_fin.getDate() + 1);
        for (; fecha <= fecha_fin; fecha.setDate(fecha.getDate() + 1)) {
            if (diasSeleccionados.includes(fecha.getDay().toString())) {
                var hora_fin = new Date((fecha.toISOString().split('T')[0]) + 'T' + horaFin);
                var hora = new Date(fecha.toISOString().split('T')[0] + 'T' + horaInicio);
                while (hora < hora_fin) {
                    if ((hora.getTime() + intervalo * 6000) < hora_fin.getTime() - intervalo * 6000) {
                        if (!fechaHoraProgramada(fecha, hora)) {
                            const fechaClon = new Date(fecha);
                            const horaClon = new Date(hora);
                            agregarHorario(fechaClon, horaClon, intervalo);
                            const fila = {
                                fecha: fechaClon.toLocaleDateString(),
                                hora: horaClon.toLocaleTimeString(),
                                dia: obtenerDiaSemana(fechaClon),
                                duracion: intervalo
                            };
                            agregarFilaTabla(fila);
                            fechasHorasProgramadas.push({ fecha: fechaClon, hora: horaClon, duracion: intervalo});
                            hora.setMinutes(hora.getMinutes() + intervalo);
                        } else {
                            alert("Error: La hora y fecha se cruza con otro registro.");
                            return;
                        }
                    } else {
                        break;
                    }
                }
            }
        }
    }
}

function fechaHoraProgramada(fecha, hora) {
    return fechasHorasProgramadas.some(function (item) {
        const itemFecha = item.fecha.toLocaleDateString();
        const itemHora = item.hora.toLocaleTimeString();
        const nuevaFecha = fecha.toLocaleDateString();
        const nuevaHora = hora.toLocaleTimeString();

        return itemFecha === nuevaFecha && itemHora === nuevaHora;
    });
}

function agregarHorario(fechaClon, horaClon, duracion) {
    var dia_ = ('0' + fechaClon.getDate()).slice(-2);
    var mes_ = ('0' + (fechaClon.getMonth() + 1)).slice(-2);
    var año_ = fechaClon.getFullYear();
    var fechaFormateada = año_ + '-' + mes_ + '-' + dia_;
    var Dia = {
        Pk_dia: parseInt(fechaClon.getDay())
    };
    var Recurso = {
        Pk_recurso: parseInt(recurso)
    };
    var horario = {
        Fecha: fechaFormateada,
        Hora_inicio: horaClon.toLocaleTimeString(),
        duracion: duracion,
        Costo: obtenerCosto(),
        Fk_dia: Dia,
        Fk_recurso: Recurso
    };
    horarios.push(horario);
}

function obtenerCosto() {
    var costo_t = parseInt(document.getElementById('costo').value);
    var costo = 0;
    if (!isNaN(costo_t)) {
        costo = costo_t;
    }
    return costo;
}

function agregarFilaTabla(fila) {
    const tabla = document.getElementById('agenda');
    const tbody = tabla.getElementsByTagName('tbody')[0];
    const costo = obtenerCosto();

    const nuevaFila = tbody.insertRow();
    const celdaDia = nuevaFila.insertCell(0);
    const celdaFecha = nuevaFila.insertCell(1);
    const celdaHora = nuevaFila.insertCell(2);
    const celdaDur = nuevaFila.insertCell(3);
    const celdaCosto = nuevaFila.insertCell(4);
    const celdaBtn = nuevaFila.insertCell(5);

    celdaDia.textContent = fila.dia;
    celdaFecha.textContent = fila.fecha;
    celdaHora.textContent = fila.hora;
    celdaDur.textContent = fila.duracion;
    celdaCosto.textContent = costo;

    const btnEliminar = document.createElement('button');
    btnEliminar.textContent = 'Eliminar';
    btnEliminar.addEventListener('click', function () {
        eliminarFilaTabla(nuevaFila);
    });

    celdaBtn.appendChild(btnEliminar);
}

function eliminarFilaTabla(fila) {
    const tabla = document.getElementById('agenda');
    const tbody = tabla.getElementsByTagName('tbody')[0];

    const index = fila.rowIndex - 1;
    if (confirm("¿Está seguro de eliminar el horario?")) {
        tbody.removeChild(fila);
        fechasHorasProgramadas.splice(index, 1);
        horarios.splice(index, 1);
    }
}

function obtenerDiasSeleccionados() {
    var diasSeleccionados = [];
    var checkboxes = document.querySelectorAll('input[type=checkbox]:checked');

    checkboxes.forEach(function (checkbox) {
        diasSeleccionados.push(checkbox.value);
    });

    return diasSeleccionados;
}

function obtenerDiaSemana(fecha) {
    const dias = ['Lunes', 'Martes', 'Miercoles', 'Jueves', 'Viernes', 'Sabado', 'Domingo'];
    const indiceDia = fecha.getDay();
    const indiceAjustado = (indiceDia === 0) ? 6 : indiceDia - 1;
    return dias[indiceAjustado];
}


document.getElementById("confirmar").addEventListener("click", () => {
    if (horarios.length > 0) {
        fetch('/Recurso/Crear_horarios', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(horarios)
        }).then(response => response.json())
            .then(data => {
                if (data) {
                    alert("¡Los horarios se han guradado correctamente!");
                    window.location.href = 'Asignar_horario'
                } else {
                    alert("¡Error al insertar los horarios!\n¡El recurso ya existe un registro con la misma fecha y hora!");
                }
            })
            .catch(error => {
                alert("¡Error al enviar los datos al servidor!");
            });
    } else {
        alert("No ha programado los horarios.");
    }
});