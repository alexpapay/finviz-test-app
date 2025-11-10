import { useState } from "react";
import { Box, Paper, Typography } from "@mui/material";
import { SearchInput } from "@/shared/ui/SearchInput";
import { SearchResults } from "@/features/search/ui/SearchResults";
import { TreeExplorer } from "@/features/treeExplorer/ui/TreeExplorer";

export const ImageNetPage = () => {
    const [query, setQuery] = useState("");

    const handleSelectFromSearch = (id: number) => {
        // сюда можешь добавить:
        // - запрос к /api/nodes/{id}/path
        // - раскрытие нужных веток в TreeExplorer через состояние/контекст
        console.log("Selected from search:", id);
    };

    return (
        <Box display="flex" height="100vh" bgcolor="background.default" color="text.primary">
            <Box width={360} p={2} component={Paper} square sx={{ borderRight: 1, borderColor: "divider" }}>
                <Typography variant="h6" gutterBottom>
                    Search
                </Typography>
                <SearchInput value={query} onChange={setQuery} placeholder="Search nodes..." />
                <Box mt={2}>
                    <SearchResults query={query} onSelect={handleSelectFromSearch} />
                </Box>
            </Box>

            <Box flex={1} p={2}>
                <Typography variant="h6" gutterBottom>
                    ImageNet Tree
                </Typography>
                <TreeExplorer onSelectNode={(id) => console.log("Clicked in tree:", id)} />
            </Box>
        </Box>
    );
};
