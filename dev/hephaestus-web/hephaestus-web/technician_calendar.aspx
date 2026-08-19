<%@ Page Title="Calendário de Técnicos" Language="C#" MasterPageFile="~/hephaestus.Master" AutoEventWireup="true" CodeBehind="technician_calendar.aspx.cs" Inherits="hephaestus_web.technician_calendar" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="calendar-page">
        <div class="d-flex flex-wrap align-items-center justify-content-between mb-4">
            <div class="d-flex align-items-center mb-3 mb-md-0">
                <span class="page-title-icon mr-3"><i class="mdi mdi-calendar-clock"></i></span>
                <div>
                    <h3 class="mb-1">Calendário de Técnicos</h3>
                    <p class="text-muted mb-0">Agenda diária de intervenções por técnico e localidade</p>
                </div>
            </div>
            <div class="calendar-toolbar d-flex flex-wrap align-items-center">
                <select id="locationFilter" class="form-control" aria-label="Filtrar por localidade">
                    <option value="">Todas as localidades</option>
                </select>
                <div class="date-navigation">
                    <button type="button" id="previousDay" class="btn btn-dark" title="Dia anterior"><i class="mdi mdi-chevron-left"></i></button>
                    <input type="date" id="calendarDate" class="form-control" aria-label="Data da agenda" />
                    <button type="button" id="nextDay" class="btn btn-dark" title="Dia seguinte"><i class="mdi mdi-chevron-right"></i></button>
                </div>
                <button type="button" id="todayButton" class="btn btn-outline-success">Hoje</button>
            </div>
        </div>
        <div class="row mb-4">
            <div class="col-md-4 mb-3 mb-md-0">
                <div class="card calendar-stat h-100"><span class="text-muted small">Técnicos visíveis</span><span id="technicianCount" class="value mt-2">0</span></div>
            </div>
            <div class="col-md-4 mb-3 mb-md-0">
                <div class="card calendar-stat info h-100"><span class="text-muted small">Intervenções agendadas</span><span id="taskCount" class="value mt-2">0</span></div>
            </div>
            <div class="col-md-4">
                <div class="card calendar-stat warning h-100"><span class="text-muted small">Horas planeadas</span><span id="plannedHours" class="value mt-2">0h</span></div>
            </div>
        </div>
        <div class="card">
            <div class="card-body p-0">
                <div class="d-flex flex-wrap justify-content-between align-items-center px-4 py-3 border-bottom">
                    <h4 id="selectedDateLabel" class="card-title mb-2 mb-md-0">Agenda</h4>
                    <div class="legend"><span class="legend-open">Open</span><span class="legend-progress">In Progress</span><span class="legend-hold">On Hold</span><span class="legend-done">Done</span></div>
                </div>
                <div class="schedule-scroll">
                    <div id="schedule" class="schedule" aria-live="polite"></div>
                </div>
            </div>
        </div>
    </div>
    <script>
        (function() {
            'use strict';
            var technicians = window.hephaestusTechnicians || [];
            var tasks = window.hephaestusTasks || [];
            var dateInput = document.getElementById('calendarDate');
            var locationFilter = document.getElementById('locationFilter');
            var baseDate = new Date();
            baseDate.setHours(0, 0, 0, 0);

            function iso(date) {
                var y = date.getFullYear(),
                    m = String(date.getMonth() + 1).padStart(2, '0'),
                    d = String(date.getDate()).padStart(2, '0');
                return y + '-' + m + '-' + d;
            }
            dateInput.value = iso(baseDate);

            function minutes(time) {
                var p = time.split(':');
                return (+p[0] * 60) + (+p[1]);
            }

            function escapeHtml(value) {
                var div = document.createElement('div');
                div.textContent = value;
                return div.innerHTML;
            }

            function moveDate(days) {
                var d = new Date(dateInput.value + 'T12:00:00');
                d.setDate(d.getDate() + days);
                dateInput.value = iso(d);
                render();
            }

            function render() {
                var selected = dateInput.value,
                    location = locationFilter.value;
                var visibleTechnicians = technicians.filter(function(t) {
                    return !location || t.location === location;
                });
                var visibleTasks = tasks.filter(function(t) {
                    return t.date === selected && visibleTechnicians.some(function(tech) {
                        return tech.id === t.technician;
                    });
                });
                var selectedDate = new Date(selected + 'T12:00:00');
                document.getElementById('selectedDateLabel').textContent = selectedDate.toLocaleDateString('pt-PT', {
                    weekday: 'long',
                    day: '2-digit',
                    month: 'long',
                    year: 'numeric'
                });
                document.getElementById('technicianCount').textContent = visibleTechnicians.length;
                document.getElementById('taskCount').textContent = visibleTasks.length;
                var hours = visibleTasks.reduce(function(sum, t) {
                    return sum + minutes(t.end) - minutes(t.start);
                }, 0) / 60;
                document.getElementById('plannedHours').textContent = hours.toLocaleString('pt-PT', {
                    maximumFractionDigits: 1
                }) + 'h';
                var html = '<div class="schedule-header"><div>Técnico</div>';
                for (var h = 8; h < 18; h++) html += '<div>' + String(h).padStart(2, '0') + ':00</div>';
                html += '</div>';
                Array.from(new Set(visibleTechnicians.map(function(t) {
                    return t.location;
                }))).sort().forEach(function(place) {
                    var localTechs = visibleTechnicians.filter(function(t) {
                        return t.location === place;
                    });
                    if (!localTechs.length) return;
                    html += '<div class="location-heading"><i class="mdi mdi-map-marker"></i>' + place + '<span class="text-muted ml-2 small">' + localTechs.length + ' técnico(s)</span></div>';
                    localTechs.forEach(function(tech) {
                        var techTasks = visibleTasks.filter(function(t) {
                            return t.technician === tech.id;
                        });
                        html += '<div class="technician-row"><div class="technician-cell"><span class="technician-avatar">' + tech.initials + '</span><div><div class="technician-name">' + escapeHtml(tech.name) + '</div><div class="technician-state">' + (techTasks.length ? techTasks.length + ' intervenção(ões)' : 'Disponível') + '</div></div></div>';
                        for (var i = 0; i < 10; i++) html += '<div class="time-cell" style="grid-column:' + (i + 2) + '"></div>';
                        techTasks.forEach(function(task) {
                            var start = Math.max(minutes(task.start), 480),
                                end = Math.min(minutes(task.end), 1080);
                            var columnStart = 2 + Math.floor((start - 480) / 60),
                                span = Math.max(1, Math.ceil((end - start) / 60));
                            html += '<div class="task-card task-' + task.status + '" style="grid-column:' + columnStart + ' / span ' + span + '" title="' + escapeHtml(task.code + ' — ' + task.title) + '"><div class="task-time">' + task.start + '–' + task.end + ' · ' + task.code + '</div><div class="task-title">' + escapeHtml(task.title) + '</div></div>';
                        });
                        html += '</div>';
                    });
                });
                if (!visibleTechnicians.length) html += '<div class="empty-calendar"><i class="mdi mdi-calendar-remove d-block h2"></i>Não existem técnicos disponíveis para apresentar.</div>';
                document.getElementById('schedule').innerHTML = html;
            }
            document.getElementById('previousDay').addEventListener('click', function() {
                moveDate(-1);
            });
            document.getElementById('nextDay').addEventListener('click', function() {
                moveDate(1);
            });
            document.getElementById('todayButton').addEventListener('click', function() {
                dateInput.value = iso(baseDate);
                render();
            });
            dateInput.addEventListener('change', render);
            Array.from(new Set(technicians.map(function(t) {
                return t.location;
            }))).sort().forEach(function(location) {
                var option = document.createElement('option');
                option.value = location;
                option.textContent = location;
                locationFilter.appendChild(option);
            });
            locationFilter.addEventListener('change', render);
            render();
        }());
    </script>
</asp:Content>