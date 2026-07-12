<script lang="ts">
    import { ChevronLeft, Languages, Mail, MapPin, ShieldAlert, Copyright } from '@lucide/svelte';

    let lang = $state<'en' | 'de'>('en');

    function toggleLang() {
        lang = lang === 'en' ? 'de' : 'en';
    }

    const content = {
        en: {
            title: 'Legal Notice',
            subtitle: 'Information according to § 5 DDG / § 18 MStV',
            sections: [
                {
                    title: 'Operator',
                    body: `<strong>Max Mustermann</strong><br>Musterstraße 12<br>12345 Musterstadt<br>Germany`
                },
                {
                    title: 'Contact',
                    body: `Email: max.mustermann@musterschule-beispiel.de`
                },
                {
                    title: 'Responsible for content',
                    body: `According to § 18 Abs. 2 MStV:<br><strong>Max Mustermann</strong>, Musterstraße 12, 12345 Musterstadt`
                },
                {
                    title: 'School project context',
                    body: `This website is a non-commercial school project created for educational and testing purposes. It is not operated for profit, and no goods or services are sold or advertised.<br><br><strong>Supervising institution (optional):</strong><br>Musterschule Musterstadt, Schulstraße 1, 12345 Musterstadt<br>Represented by: [Name of teacher]`
                },
                {
                    title: 'Disclaimer',
                    body: `The content of this website has been created with care, but we cannot guarantee accuracy, completeness, or timeliness. As a school project, this site may not meet the standards of a professional website. Links to external websites are not controlled by us and we assume no liability for their content.`
                },
                {
                    title: 'Copyright',
                    body: `Unless otherwise stated, content created by the project author is subject to copyright. Reproduction, distribution, or use outside the scope of this school project requires prior consent.`
                }
            ],
            notice: 'The name, address, and email shown above are placeholder (dummy) data and must be replaced with real information before any public deployment.'
        },
        de: {
            title: 'Impressum',
            subtitle: 'Angaben gemäß § 5 DDG / § 18 MStV',
            sections: [
                {
                    title: 'Betreiber',
                    body: `<strong>Max Mustermann</strong><br>Musterstraße 12<br>12345 Musterstadt<br>Deutschland`
                },
                {
                    title: 'Kontakt',
                    body: `E-Mail: max.mustermann@musterschule-beispiel.de`
                },
                {
                    title: 'Verantwortlich für den Inhalt',
                    body: `Gemäß § 18 Abs. 2 MStV:<br><strong>Max Mustermann</strong>, Musterstraße 12, 12345 Musterstadt`
                },
                {
                    title: 'Kontext: Schulprojekt',
                    body: `Diese Website ist ein nicht-kommerzielles Schulprojekt zu Lern- und Testzwecken. Es werden keine Gewinne erzielt und keine Waren oder Dienstleistungen verkauft oder beworben.<br><br><strong>Betreuende Institution (optional):</strong><br>Musterschule Musterstadt, Schulstraße 1, 12345 Musterstadt<br>Vertreten durch: [Name der Lehrkraft]`
                },
                {
                    title: 'Haftungsausschluss',
                    body: `Die Inhalte wurden mit Sorgfalt erstellt, für Richtigkeit, Vollständigkeit und Aktualität kann jedoch keine Gewähr übernommen werden. Als Schulprojekt erfüllt diese Website möglicherweise nicht die Standards einer professionellen Website. Für Inhalte verlinkter externer Websites übernehmen wir keine Haftung.`
                },
                {
                    title: 'Urheberrecht',
                    body: `Sofern nicht anders angegeben, unterliegen die durch den Projektautor erstellten Inhalte dem Urheberrecht. Vervielfältigung, Verbreitung oder Nutzung außerhalb des Schulprojekts bedürfen der vorherigen Zustimmung.`
                }
            ],
            notice: 'Name, Adresse und E-Mail oben sind Platzhalter (Dummy-Daten) und müssen vor einer echten Veröffentlichung durch reale Angaben ersetzt werden.'
        }
    };
</script>

<div class="mx-auto flex w-full items-center justify-between md:w-200">
    <button class="btn mt-4 mb-4 ml-4 btn-lg" aria-label="Navigate Back" onclick={() => history.back()}>
        <ChevronLeft />
    </button>

    <button class="btn mt-4 mb-4 mr-4 btn-sm gap-2" aria-label="Toggle language" onclick={toggleLang}>
        <Languages size={16} />
        {lang === 'en' ? 'Deutsch' : 'English'}
    </button>
</div>

<div class="mx-auto mb-8 w-fit text-center">
    <h1 class="text-3xl font-bold">{content[lang].title}</h1>
    <p class="mt-1 text-sm text-base-content/60">{content[lang].subtitle}</p>
</div>

<div class="mx-auto flex w-full flex-col gap-6 p-4 md:w-200">
    <div class="card border border-base-300 bg-base-100 shadow-sm">
        <div class="card-body gap-0 p-0">
            {#each content[lang].sections as section, i}
                <div class="p-6">
                    <h2 class="badge badge-outline badge-sm mb-2 font-semibold">{section.title}</h2>
                    <p class="text-sm leading-relaxed text-base-content/80">{@html section.body}</p>
                </div>
                {#if i < content[lang].sections.length - 1}
                    <div class="divider m-0"></div>
                {/if}
            {/each}
        </div>
    </div>

    <div class="alert alert-warning">
        <ShieldAlert size={20} />
        <span class="text-sm">{content[lang].notice}</span>
    </div>
</div>